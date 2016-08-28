import * as d3 from 'd3';

// shamelessly stolen from:
//    http://bl.ocks.org/maybelinot/5552606564ef37b5de7e47ed2b7dc099
//    http://jsfiddle.net/Hq4ef/
let width = 960;
let height = 700;
let totalCount = 0;
let radius = (Math.min(width, height) / 2) - 10;
let formatNumber = d3.format(',d');
let formatPercent = d3.format('.2%');
let svg = null;

// Breadcrumb dimensions: width, height, spacing, width of tip/tail.
let b = {
    w: 240, h: 30, s: 3, t: 10
};

let x = d3.scaleLinear().range([0, 2 * Math.PI]);
let y = d3.scaleSqrt().range([0, radius]);
let colors = d3.scaleOrdinal(d3.schemeCategory20);
let partition = d3.partition();

let arc = d3.arc()
    .startAngle(function(d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x0))); })
    .endAngle(function(d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x1))); })
    .innerRadius(function(d) { return Math.max(0, y(d.y0)); })
    .outerRadius(function(d) { return Math.max(0, y(d.y1)); });

function statChildren(d): string {
    return d.categoryStats;
}

function getNodeName(d): string {
    let name = d.data.name;

    if(d.data.year) {
        name = d.data.year.toString();
    }

    return name;
}

function getCategoryCount(d): number {
    let count = 0;

    // all photos will have many children with children - we want to count them
    if(d.children) {
        if(d.children[0].children) {
            for(let child of d.children) {
                count += child.children.length;
            }
        }
        else {
            count = d.children.length;
        }
    }

    return count;
}

function getNodeTitle(d): string {
    let titleParts: Array<string> = [];
    let cats = getCategoryCount(d);

    titleParts.push(getNodeName(d));

    if(cats > 0) {
        titleParts.push(`Categories: ${formatNumber(cats)}`);
    }

    titleParts.push(`Photos: ${formatNumber(d.value)} (${formatPercent(d.value / totalCount)})`);

    return titleParts.join('\n');
}

function initSunburst(): void {
    svg = d3.select('#sunburst')
        .append('svg')
            .attr('width', width)
            .attr('height', height)
            .attr('class', 'center-block')
        .append('g')
            .attr('id', 'container')
            .attr('transform', 'translate(' + width / 2 + ',' + (height / 2) + ')')
            .on("mouseleave", mouseleave);

    svg.append('svg:circle')
        .attr('r', radius)
        .style('opacity', 0);
}

function initBreadcrumbTrail(): void {
    d3.select('#breadcrumbs')
        .append('svg:svg')
            .attr('width', width)
            .attr('height', 36)
            .attr('id', 'trail');
}

function breadcrumbPoints(d, i: number): string {
    let points: Array<string> = [];
    points.push('0,0');
    points.push(b.w + ',0');
    points.push(b.w + b.t + ',' + (b.h / 2));
    points.push(b.w + ',' + b.h);
    points.push('0,' + b.h);

    if (i > 0) { // Leftmost breadcrumb; don't include 6th vertex.
        points.push(b.t + ',' + (b.h / 2));
    }

    return points.join(' ');
}

function updateBreadcrumbs(nodeArray): void {
    // Data join; key function combines name and depth (= position in sequence).
    let g = d3.select('#trail')
        .selectAll('g')
        .data(nodeArray, function(d) {
            return d.depth;
        });

    // Add breadcrumb and label for entering nodes.
    let entering = g.enter().append('svg:g');

    entering.append('svg:polygon')
        .attr('points', breadcrumbPoints)
        .style('fill', function(d) {
            return colors(getNodeName(d));
        });

    entering.append('svg:text')
        .attr('x', (b.w + b.t) / 2)
        .attr('y', b.h / 2)
        .attr('dy', '0.35em')
        .attr('text-anchor', 'middle')
        .text(function(d) {
            return getNodeName(d);
        });

    // Set position for entering and updating nodes.
    g.attr('transform', function(d, i: number) {
        return 'translate(' + i * (b.w + b.s) + ', 0)';
    });

    // Remove exiting nodes.
    g.exit().remove();
}

function updateCategoryCount(count: number): void {
    let display = `Categories: ${getCountDisplay(count)}`;

    d3.select('#categoryCount')
        .text(display);
}

function updatePhotoCount(count: number): void {
    let display = 'Photos: ';

    if(count === 0) {
        display += 'n/a';
    }
    else {
        display += `${getCountDisplay(count)} (${formatPercent(count / totalCount)})`;
    }

    d3.select('#photoCount')
        .text(display);
}

function getCountDisplay(count: number): string {
    if(count > 0) {
        return formatNumber(count);
    }

    return 'n/a';
}

function mouseover(d): void {
    let sequenceArray = d.ancestors().reverse();
    updateBreadcrumbs(sequenceArray);

    updateCategoryCount(getCategoryCount(d));
    updatePhotoCount(d.value);

    // Fade all the segments.
    d3.selectAll('path')
        .style('opacity', 0.3);

    // Then highlight only those that are an ancestor of the current segment.
    svg.selectAll('path')
        .filter(function(node) {
            return (sequenceArray.indexOf(node) >= 0);
        })
        .style('opacity', 1);
}

function mouseleave(d): void {
    updateCategoryCount(0);
    updatePhotoCount(0);

    d3.selectAll('path')
        .style('opacity', 1);
}

function click(d): void {
    if(!d.children) {
        d = d.parent;
    }

    svg.transition()
        .duration(750)
        .tween('scale', function() {
            let xd = d3.interpolate(x.domain(), [d.x0, d.x1]);
            let yd = d3.interpolate(y.domain(), [d.y0, 1]);
            let yr = d3.interpolate(y.range(), [d.y0 ? 20 : 0, radius]);

            return function(t) {
                x.domain(xd(t));
                y.domain(yd(t)).range(yr(t));
            };
        })
        .selectAll('path')
        .attrTween('d', function(d) {
            return function() { return arc(d); };
        });
}

initBreadcrumbTrail();
updateCategoryCount(0);
updatePhotoCount(0);
initSunburst();

d3.json('/api/photos/getStats', function(error, root) {
    if (error) throw error;

    root = { name: 'All Photos', categoryStats: root };
    root = d3.hierarchy(root, statChildren);

    root.sum(function(d) {
        return d.photoCount;
    });

    let data = partition(root).descendants();

    totalCount = root.value;

    svg.selectAll('path')
        .data(data)
        .enter()
        .append('path')
            .attr('d', arc)
            .style('fill', function(d) { return colors(getNodeName(d)); })
            .on('click', click)
            .on('mouseover', mouseover)
        .append('title')
            .text(function(d) {
                return getNodeTitle(d);
            });
});

d3.select(self.frameElement)
    .style('height', height + 'px');
