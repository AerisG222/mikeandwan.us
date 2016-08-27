/// <reference path="./typings/index.d.ts" />
// shamelessly stolen from:
//    http://bl.ocks.org/maybelinot/5552606564ef37b5de7e47ed2b7dc099
//    http://jsfiddle.net/Hq4ef/
let width = 960;
let height = 700;
let radius = (Math.min(width, height) / 2) - 10;

let formatNumber = d3.format(',d');

// Breadcrumb dimensions: width, height, spacing, width of tip/tail.
var b = {
    w: 75, h: 30, s: 3, t: 10
};

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

let svg = d3.select('#stats')
    .append('svg')
        .attr('width', width)
        .attr('height', height)
        .attr('class', 'center-block')
    .append('g')
        .attr('transform', 'translate(' + width / 2 + ',' + (height / 2) + ')');

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

function initializeBreadcrumbTrail() {
  // Add the svg area.
  var trail = d3.select("#sequence").append("svg:svg")
      .attr("width", width)
      .attr("height", 50)
      .attr("id", "trail");
  // Add the label at the end, for the percentage.
  trail.append("svg:text")
    .attr("id", "endlabel")
    .style("fill", "#000");
}

function breadcrumbPoints(d, i) {
    var points = [];
    points.push("0,0");
    points.push(b.w + ",0");
    points.push(b.w + b.t + "," + (b.h / 2));
    points.push(b.w + "," + b.h);
    points.push("0," + b.h);

    if (i > 0) { // Leftmost breadcrumb; don't include 6th vertex.
      points.push(b.t + "," + (b.h / 2));
    }

    return points.join(" ");
}

// Update the breadcrumb trail to show the current sequence and percentage.
function updateBreadcrumbs(nodeArray, percentageString) {
  // Data join; key function combines name and depth (= position in sequence).
  var g = d3.select("#trail")
      .selectAll("g")
      .data(nodeArray, function(d) { return d.name + d.depth; });

  // Add breadcrumb and label for entering nodes.
  var entering = g.enter().append("svg:g");

  entering.append("svg:polygon")
      .attr("points", breadcrumbPoints)
      .style("fill", function(d) { return colors(d.name); });

  entering.append("svg:text")
      .attr("x", (b.w + b.t) / 2)
      .attr("y", b.h / 2)
      .attr("dy", "0.35em")
      .attr("text-anchor", "middle")
      .text(function(d) { return d.name; });

  // Set position for entering and updating nodes.
  g.attr("transform", function(d, i) {
    return "translate(" + i * (b.w + b.s) + ", 0)";
  });

  // Remove exiting nodes.
  g.exit().remove();

  // Now move and update the percentage at the end.
  d3.select("#trail").select("#endlabel")
      .attr("x", (nodeArray.length + 0.5) * (b.w + b.s))
      .attr("y", b.h / 2)
      .attr("dy", "0.35em")
      .attr("text-anchor", "middle")
      .text(percentageString);

  // Make the breadcrumb trail visible, if it's hidden.
  d3.select("#trail")
      .style("visibility", "");
}

d3.json('/api/photos/getStats', function(error, root) {
    if (error) throw error;

    root = { name: 'All Photos', categoryStats: root };
    root = d3.hierarchy(root, statChildren);

    root.sum(function(d) {
        return d.photoCount;
    });

    let data = partition(root).descendants();

    svg.selectAll('path')
        .data(data)
        .enter()
        .append('path')
            .attr('d', arc)
            .style('fill', function(d) { return color(getNodeName(d.children ? d : d.parent)); })
            .on('click', click)
            .on("mouseover", mouseover)
        .append('title')
            .text(function(d) {
                return getNodeTitle(d);
            });

    svg.selectAll('text')
        .data(data)
        .enter()
        .append('text')
            .attr('x', function(d) { d.x })
            .attr('y', function(d) { d.y })
            .text(function(d) {
                return getNodeName(d);
            });
});

// Fade all but the current sequence, and show it in the breadcrumb trail.
function mouseover(d) {

  var percentage = (100 * d.value / totalSize).toPrecision(3);
  var percentageString = percentage + "%";
  if (percentage < 0.1) {
    percentageString = "< 0.1%";
  }

  d3.select("#percentage")
      .text(percentageString);

  d3.select("#explanation")
      .style("visibility", "");

  var sequenceArray = getAncestors(d);
  updateBreadcrumbs(sequenceArray, percentageString);

  // Fade all the segments.
  d3.selectAll("path")
      .style("opacity", 0.3);

  // Then highlight only those that are an ancestor of the current segment.
  vis.selectAll("path")
      .filter(function(node) {
                return (sequenceArray.indexOf(node) >= 0);
              })
      .style("opacity", 1);
}

// Restore everything to full opacity when moving off the visualization.
function mouseleave(d) {

  // Hide the breadcrumb trail
  d3.select("#trail")
      .style("visibility", "hidden");

  // Deactivate all segments during transition.
  d3.selectAll("path").on("mouseover", null);

  // Transition each segment to full opacity and then reactivate it.
  d3.selectAll("path")
      .transition()
      .duration(1000)
      .style("opacity", 1)
      .each("end", function() {
              d3.select(this).on("mouseover", mouseover);
            });

  d3.select("#explanation")
      .transition()
      .duration(1000)
      .style("visibility", "hidden");
}

function click(d) {
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

d3.select(self.frameElement)
    .style('height', height + 'px');
