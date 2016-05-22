var gulp = require("gulp"),
    lessc = require("gulp-less"),
    uglify = require("gulp-uglify"),
    rename = require("gulp-rename"),
    minifyCss = require('gulp-minify-css'),
    runSequence = require("run-sequence"),
    sourcemaps = require("gulp-sourcemaps"),
    del = require("del"),
    eslint = require("gulp-eslint");

var paths = {
    wwwroot: "ServiceUnits/**/wwwroot/",
    pages: "ServiceUnits/*/*/+(Users|Anonymous|Admin)/Pages/",
};
paths.scripts = paths.wwwroot + "Scripts/**/*.js";
paths.minScripts = paths.wwwroot + "Scripts/**/*.min.js";
paths.pageScripts = paths.pages + "**/*.js";
paths.minPageScripts = paths.pages + "**/*.min.js";
paths.lesses = paths.wwwroot + "Content/**/*.less";
paths.pageLesses = paths.pages + "**/*.less";
paths.css = paths.wwwroot + "Content/**/*.css";
paths.pageCss = paths.pages + "**/*.css";
paths.minCss = paths.wwwroot + "Content/**/*.min.css";
paths.minPageCss = paths.pages + "**/*.min.css";

gulp.task("clean:min", function () {
    return del([
        paths.minScripts,
        paths.minPageScripts,
        paths.minCss,
        paths.minPageCss
    ]);
});
gulp.task("clean:css", function () {
    return del([paths.css, paths.pageCss]);
});

gulp.task("lint", function () {
    var argv = require("yargs").argv;
    var format = "ServiceUnits/{serviceUnitName}/*/**/Pages/{file}.js"
    var targets = [format.replace("{serviceUnitName}", "*").replace("{file}", "*")];
    if (argv.su) {
        targets = (argv.su += "").split(",").map(function (item) {
            return format.replace("{serviceUnitName}", item);
        });
    }
    var file = argv.file ? argv.file : "*";
    targets = targets.map(function (item) {
        return item.replace("{file}", file);
    });
    targets = targets.concat(targets.map(function(item){
        return "!" + item.replace(".js", ".min.js");
    }));
    return gulp.src(targets)
        .pipe(eslint({ useEslintrc: true }))
        .pipe(eslint.format())
        .pipe(eslint.failAfterError());
});

gulp.task("minify-js", function () {
    return gulp.src([
                paths.scripts,
                paths.pageScripts,
                "!" + paths.minScripts,
                "!" + paths.minPageScripts
    ], { base: "./" })
            //.pipe(sourcemaps.init())
            .pipe(uglify())
            //.pipe(sourcemaps.write("./"))
            .pipe(rename({
                suffix: ".min"
            }))
            .pipe(gulp.dest("./"));
});

gulp.task("less", function () {
    return gulp.src([
        paths.lesses, paths.pageLesses
    ], { base: "./" })
        .pipe(lessc())
        .pipe(gulp.dest("./"));
});

gulp.task("minify-css", function () {
    return gulp.src([
        paths.css,
        paths.pageCss,
        "!" + paths.minCss,
        "!" + paths.minPageCss
    ], { base: "./" })
    .pipe(minifyCss())
    .pipe(rename({
        suffix: ".min"
    }))
    .pipe(gulp.dest("./"));
});

gulp.task("clean", function (cb) {
    runSequence(
        ["clean:min", "clean:css"],
        cb
    );
});

gulp.task("default", function (cb) {
    runSequence(
        ["clean"],
        ["minify-js", "less"],
        ["minify-css"],
        cb
    );
});