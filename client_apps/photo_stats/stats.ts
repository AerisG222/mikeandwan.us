/// <reference path="./typings/index.d.ts" />
// shamelessly stolen from: http://bl.ocks.org/maybelinot/5552606564ef37b5de7e47ed2b7dc099
let width = 960;
let height = 700;
let radius = (Math.min(width, height) / 2) - 10;

let formatNumber = d3.format(",d");

let x = d3.scaleLinear()
    .range([0, 2 * Math.PI]);

let y = d3.scaleSqrt()
    .range([0, radius]);

let color = d3.scaleOrdinal(d3.schemeCategory10);

let partition = d3.partition();

let arc = d3.arc()
    .startAngle(function(d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x0))); })
    .endAngle(function(d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x1))); })
    .innerRadius(function(d) { return Math.max(0, y(d.y0)); })
    .outerRadius(function(d) { return Math.max(0, y(d.y1)); });

let svg = d3.select("#stats").append("svg")
    .attr("width", width)
    .attr("height", height)
    .attr("class", "center-block")
    .append("g")
    .attr("transform", "translate(" + width / 2 + "," + (height / 2) + ")");

let statChildren = function(d) {
    return d.categoryStats;
}

let getNodeName = function(d) {
    let name = d.data.name;

    if(d.data.year) {
        name = d.data.year.toString();
    }

    return name;
}

let getCategoryCount = function(d) {
    if(d.children) {
        return d.children.length;
    }

    return 0;
}

let getNodeTitle = function(d) {
    let titleParts: Array<string> = [];
    let cats = getCategoryCount(d);

    titleParts.push(getNodeName(d));

    if(cats > 0) {
        titleParts.push(`Categories: ${formatNumber(cats)}`);
    }

    titleParts.push(`Photos: ${formatNumber(d.value)}`);

    return titleParts.join('\n');
}

d3.json("/api/photos/getStats", function(error, root) {
    if (error) throw error;

    root = { name: 'All Photos', categoryStats: root };
    root = d3.hierarchy(root, statChildren);

    root.sum(function(d) {
        return d.photoCount;
    });

    svg.selectAll("path")
        .data(partition(root).descendants())
        .enter().append("path")
            .attr("d", arc)
            .style("fill", function(d) { return color(getNodeName(d.children ? d : d.parent)); })
            .on("click", click)
        .append("title")
            .text(function(d) {
                return getNodeTitle(d);
            });
});

function click(d) {
    svg.transition()
        .duration(750)
        .tween("scale", function() {
            let xd = d3.interpolate(x.domain(), [d.x0, d.x1]);
            let yd = d3.interpolate(y.domain(), [d.y0, 1]);
            let yr = d3.interpolate(y.range(), [d.y0 ? 20 : 0, radius]);

            return function(t) {
                x.domain(xd(t));
                y.domain(yd(t)).range(yr(t));
            };
        })
        .selectAll("path")
        .attrTween("d", function(d) {
            return function() { return arc(d); };
        });
}

d3.select(self.frameElement)
    .style("height", height + "px");
