var gulp = require("gulp"),
    rimraf = require("rimraf"),
	fs = require("fs"),
    shell = require("gulp-shell"),
    runSequence = require('run-sequence');

var bowerDef = JSON.parse(fs.readFileSync("./bower.json"));

var paths = {
        bower: "./bower_components/",
        lib: "./wwwroot/lib/"
    };

//bower tasks

gulp.task("clean-lib", function (cb) {
    rimraf(paths.lib, cb);
});

gulp.task("clean-bower", function (cb) {
    rimraf(paths.bower, cb);
});

gulp.task("install-bower", shell.task(["bower install"]));

gulp.task("copy-bower", function (cb) {
    var keys = Object.keys(bowerDef.dependencies),
        counter = 0,
        end = function(){
            counter++;
            if(keys.length <= counter){
                cb();
            }
        };  
    keys.forEach(function (key) {
        var path = key === "json3" ? "lib" :
                   key === "respond" ? "dest" :
                   key === "jasmine" ? "lib" :
                   key === "jasmine-jquery" ? "lib" :
                   "dist";

        var destFile = paths.bower + key + "/" + path + "/**/*.*";
        gulp.src(destFile).pipe(gulp.dest(paths.lib + key)).on("end", end);
    });    
});

gulp.task("bower", function(cb){
    runSequence(
        ["clean-bower", "clean-lib"],
        "install-bower",
        "clean-lib",
        "copy-bower",
        "clean-bower",
        cb
    );
});