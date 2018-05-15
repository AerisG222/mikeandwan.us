// shamelessly stolen from:
//    http://bl.ocks.org/maybelinot/5552606564ef37b5de7e47ed2b7dc099
//    http://jsfiddle.net/Hq4ef/
import * as d3 from 'd3';

import { AuthService } from './auth-service';
import { Config } from './config';

declare var __API_URL__: string;

export class PhotoStats {
    private readonly _config = new Config();

    private _authService = new AuthService(this._config);
    width = 960;
    height = 700;
    totalCount = 0;
    radius = (Math.min(this.width, this.height) / 2) - 10;
    formatNumber = d3.format(',d');
    formatPercent = d3.format('.2%');
    svg: any = null;

    // Breadcrumb dimensions: width, height, spacing, width of tip/tail.
    b = {
        w: 240, h: 30, s: 3, t: 10
    };

    x = d3.scaleLinear().range([0, 2 * Math.PI]);
    y = d3.scaleSqrt().range([0, this.radius]);
    colors = d3.scaleOrdinal(d3.schemeCategory10);

    arc = d3.arc()
        .startAngle((d: any) => Math.max(0, Math.min(2 * Math.PI, this.x(d.x0))))
        .endAngle((d: any) => Math.max(0, Math.min(2 * Math.PI, this.x(d.x1))))
        .innerRadius((d: any) => Math.max(0, this.y(d.y0)))
        .outerRadius((d: any) => Math.max(0, this.y(d.y1)));

    statChildren(d): string[] {
        return d.categoryStats;
    }

    getNodeName(d): string {
        let name = d.data.name;

        if(d.data.year) {
            name = d.data.year.toString();
        }

        return name;
    }

    getCategoryCount(d): number {
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

    getNodeTitle(d): string {
        let titleParts: Array<string> = [];
        let cats = this.getCategoryCount(d);

        titleParts.push(this.getNodeName(d));

        if(cats > 0) {
            titleParts.push(`Categories: ${this.formatNumber(cats)}`);
        }

        titleParts.push(`Photos: ${this.formatNumber(d.value)} (${this.formatPercent(d.value / this.totalCount)})`);

        return titleParts.join('\n');
    }

    initSunburst(): void {
        this.svg = d3.select('#sunburst')
            .append('svg')
                .attr('width', this.width)
                .attr('height', this.height)
                .attr('class', 'center-block')
            .append('g')
                .attr('id', 'container')
                .attr('transform', 'translate(' + this.width / 2 + ',' + (this.height / 2) + ')')
                .on("mouseleave", d => this.mouseleave(d));

        this.svg.append('svg:circle')
            .attr('r', this.radius)
            .style('opacity', 0);
    }

    initBreadcrumbTrail(): void {
        d3.select('#breadcrumbs')
            .append('svg:svg')
                .attr('width', this.width)
                .attr('height', 36)
                .attr('id', 'trail');
    }

    breadcrumbPoints(d, i: number): string {
        let points: Array<string> = [];

        points.push('0,0');
        points.push(this.b.w + ',0');
        points.push(this.b.w + this.b.t + ',' + (this.b.h / 2));
        points.push(this.b.w + ',' + this.b.h);
        points.push('0,' + this.b.h);

        if (i > 0) { // Leftmost breadcrumb; don't include 6th vertex.
            points.push(this.b.t + ',' + (this.b.h / 2));
        }

        return points.join(' ');
    }

    updateBreadcrumbs(nodeArray): void {
        let g = d3.select('#trail')
            .selectAll('g')
            .data(nodeArray, d => `${this.getNodeName(d)}`);

        let entering = g.enter().append('svg:g');

        entering.append('svg:polygon')
            .attr('points', (d, i: number) => this.breadcrumbPoints(d, i))
            .style('fill', d => this.colors(this.getNodeName(d)));

        entering.append('svg:text')
            .attr('x', (this.b.w + this.b.t) / 2)
            .attr('y', this.b.h / 2)
            .attr('dy', '0.35em')
            .attr('text-anchor', 'middle')
            .html(d => this.getNodeName(d));

        entering.attr('transform', (d: any, i: number) => {
            return 'translate(' + d.depth * (this.b.w + this.b.s) + ', 0)';
        });

        g.exit().remove();
    }

    updateCategoryCount(count: number): void {
        let display = `Categories: ${this.getCountDisplay(count)}`;

        d3.select('#categoryCount')
            .html(display);
    }

    updatePhotoCount(count: number): void {
        let display = 'Photos: ';

        if(count === 0) {
            display += 'n/a';
        }
        else {
            display += `${this.getCountDisplay(count)} (${this.formatPercent(count / this.totalCount)})`;
        }

        d3.select('#photoCount')
            .html(display);
    }

    getCountDisplay(count: number): string {
        if(count > 0) {
            return this.formatNumber(count);
        }

        return 'n/a';
    }

    mouseover(d): void {
        let sequenceArray = d.ancestors().reverse();
        this.updateBreadcrumbs(sequenceArray);

        this.updateCategoryCount(this.getCategoryCount(d));
        this.updatePhotoCount(d.value);

        d3.selectAll('path')
            .style('opacity', 0.3)
            .filter(node => sequenceArray.indexOf(node) >= 0)
            .style('opacity', 1);
    }

    mouseleave(d): void {
        this.updateBreadcrumbs([]);
        this.updateCategoryCount(0);
        this.updatePhotoCount(0);

        d3.selectAll('path')
            .style('opacity', 1);
    }

    click(d): void {
        if(!d.children) {
            d = d.parent;
        }

        this.svg.transition()
            .duration(750)
            .tween('scale', () => {
                let xd = d3.interpolate(this.x.domain(), [d.x0, d.x1]);
                let yd = d3.interpolate(this.y.domain(), [d.y0, 1]);
                let yr = d3.interpolate(this.y.range(), [d.y0 ? 20 : 0, this.radius]);

                return t => {
                    this.x.domain(xd(t));
                    this.y.domain(yd(t)).range(yr(t));
                };
            })
            .selectAll('path')
            .attrTween('d', d => {
                return () => { return this.arc(d); }
            });
    }

    async runAsync(): Promise<void> {
        await this._authService.attemptSilentSignin();

        if (!this._authService.isLoggedIn()) {
            await this._authService.initSessionAsync();
        }

        let partition = d3.partition();

        this.initBreadcrumbTrail();
        this.updateCategoryCount(0);
        this.updatePhotoCount(0);
        this.initSunburst();

        const url = `${this._config.apiUrl}/photos/getStats`;
        const headers = new Headers({ 'Authorization': this._authService.getAuthorizationHeaderValue() });

        d3.json(url, { headers: headers }).then((root) => {
            root = { name: 'All Photos', categoryStats: root };
            root = d3.hierarchy(root, this.statChildren);

            root.sum(d => d.photoCount);

            let data = partition(root).descendants();

            this.totalCount = root.value;

            this.svg.selectAll('path')
                .data(data)
                .enter()
                .append('path')
                    .attr('d', d => this.arc(d))
                    .style('fill', d => this.colors(this.getNodeName(d)))
                    .on('click', d => this.click(d))
                    .on('mouseover', d => this.mouseover(d))
                .append('title')
                    .html(d => this.getNodeTitle(d));
        });

        d3.select(self.frameElement)
            .style('height', this.height + 'px');
    }
}
