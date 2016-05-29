var builddir = 'build';
var proddir = 'production';

var gulp = require('gulp');
var seq = require('run-sequence');
var rimraf = require('gulp-rimraf');
var cssnano = require('gulp-cssnano');
var rev = require('gulp-rev');
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');
var htmlmin = require('gulp-htmlmin');
var replace = require('gulp-replace');
var sourcemaps = require('gulp-sourcemaps');
var typescript = require('gulp-typescript');
var tslint = require('gulp-tslint');


gulp.task('watch', function () {
    gulp.watch('ts/**/*', ['moneyspin']);
});

gulp.task('allts', function (cb) {
    seq('mem',
        'bandwidth',
        'binaryclock',
        'bytecounter',
        'filesize',
        'googlemaps',
        'time',
        'countdown',
        'learning',
        'ngmaw',
        'videos',
        'photos',
        'glcube',
        'gltext',
        'moneyspin',
        cb);
});

function compileTypescript(path) {
    // copy html files (angular templates)
    gulp
        .src([path + '**/*.html'],
            { 'base': 'ts' })
        .pipe(gulp.dest('wwwroot/js'));

    var sources = [path + '**/*.ts'];
    
    // add all our typings
    sources.push('ts/typings/browser.d.ts');
    sources.push('ts/typings/pixastic/pixastic.d.ts');
    
    // compile the typescript files to js
    var tsResult = gulp
        .src(sources, { 'base': 'ts' })
        .pipe(sourcemaps.init())
        .pipe(typescript({
            target: 'es5',
            module: 'system',
            moduleResolution: 'node',
            sourceMap: true,
            emitDecoratorMetadata: true,
            experimentalDecorators: true,
            declaration: false,
            //noEmitOnError: true,
            removeComments: false,
            noLib: false,
            noImplicitAny: false,
            suppressImplicitAnyIndexErrors: true
        }));

    return tsResult
        .js
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('wwwroot/js'));
}

gulp.task('glcube', function (cb) {
    compileTypescript('ts/webgl/cube/');
    cb();
});

gulp.task('gltext', function (cb) {
    compileTypescript('ts/webgl/text/');
    cb();
});

gulp.task('mem', function (cb) {
    compileTypescript('ts/games/memory/');
    cb();
});

gulp.task('moneyspin', function (cb) {
    compileTypescript('ts/games/money_spin/');
    cb();
});

gulp.task('bandwidth', function (cb) {
    compileTypescript('ts/tools/bandwidth/');
    cb();
});

gulp.task('binaryclock', function (cb) {
    compileTypescript('ts/tools/binaryClock/');
    cb();
});

gulp.task('bytecounter', function (cb) {
    compileTypescript('ts/tools/byteCounter/');
    cb();
});

gulp.task('filesize', function (cb) {
    compileTypescript('ts/tools/filesize/');
    cb();
});

gulp.task('googlemaps', function (cb) {
    compileTypescript('ts/tools/googlemaps/');
    cb();
});

gulp.task('time', function (cb) {
    compileTypescript('ts/tools/time/');
    cb();
});

gulp.task('countdown', function (cb) {
    compileTypescript('ts/tools/weekendCountdown/');
    cb();
});

gulp.task('learning', function (cb) {
    compileTypescript('ts/tools/learning/');
    cb();
});

gulp.task('ngmaw', function (cb) {
    compileTypescript('ts/ng_maw/');
    cb();
});

gulp.task('videos', function (cb) {
    compileTypescript('ts/videos/');
    cb();
});

gulp.task('photos', function (cb) {
    compileTypescript('ts/photos/');
    cb();
});

gulp.task('tslint', function () {
    return gulp
        .src(['ts/**/*.ts', '!ts/typings/**'])
        .pipe(tslint())
        .pipe(tslint.report('verbose'));
});

gulp.task('jslibs', function(cb) {
    gulp
        .src(['wwwroot/js/libs/**/*.*'])
        .pipe(rimraf());
        
    return gulp.src(['ts/libs/**/*.*'])
        .pipe(gulp.dest('wwwroot/js/libs'));
});

gulp.task('rebuild-prod', function (cb) {
    seq('clean',
        'cp',
        'mincss',
        'pixastic',
        'modernizr',
        'update_refs',
        'final_cleanup',
        cb);
});

gulp.task('clean', function (cb) {
    gulp
        .src([builddir], {read: false})
        .pipe(rimraf());

    return gulp
        .src([proddir], {read: false})
        .pipe(rimraf());
});

gulp.task('cp', function (cb) {
    return gulp
        .src(['**',  // copy everything
            '!wwwroot/css/**/*.css',  // exclude css files (but keeping the images in the css folder)
            '!wwwroot/js/**'  // exclude the js directory for now
        ],
            { 'base': '' })
        .pipe(gulp.dest(builddir));
});

gulp.task('mincss', function (cb) {
    return gulp
        .src('wwwroot/css/**/*.css')
        .pipe(cssnano())
        .pipe(rev())
        .pipe(gulp.dest(builddir + '/wwwroot/css'))
        .pipe(rev.manifest({ merge: true }))
        .pipe(gulp.dest('.'));
});

// PIXASTIC
gulp.task('pixastic', function (cb) {
    return gulp
        .src(['wwwroot/js/libs/pixastic/pixastic.core.js',
              'wwwroot/js/libs/pixastic/histogram.js'
        ])
        .pipe(concat('pixastic.js'))
        .pipe(uglify())
        .pipe(rev())
        .pipe(gulp.dest(builddir + '/wwwroot/js/libs/pixastic'))
        .pipe(rev.manifest({ merge: true }))
        .pipe(gulp.dest('.'));
});

// UPDATE CSS/JS REFERENCES
gulp.task('update_refs', function (cb) {
    var revData = require('./rev-manifest.json');
    
    // site.css
    gulp
        .src(builddir + '/Views/Shared/_css_site.cshtml')
        .pipe(replace(/(\/css\/)site.css/g, '$1' + revData['site.css']))
        .pipe(gulp.dest(builddir + '/Views/Shared'));
    
    // pixastic
    gulp
        .src(builddir + '/Views/Shared/_js_Pixastic.cshtml')
        .pipe(replace(/(\/js\/libs\/pixastic\/)pixastic.core.js/g, '$1' + revData['pixastic.js']))
        .pipe(replace(/\n.*histogram.js.*/gmi, ''))
        .pipe(gulp.dest(builddir + '/Views/Shared'));

    cb();
});

// FINAL CLEANUP
gulp.task('final_cleanup', function (cb) {
    return gulp
        .src([builddir + '/wwwroot/**/templates.js', 'rev-manifest.json'])
        .pipe(rimraf());
});
