/// <binding BeforeBuild='copy-libs' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

'use strict';

// 
var gulp = require('gulp');

gulp.task('default', function () { });

gulp.task('copy-libs', function () {
    // Copy/Populate libs folder from dependencies
    process.stdout.write('Copy/Populate libs folder from dependencies...\n');

    gulp.src('./node_modules/font-awesome/**/*')
    .pipe(gulp.dest('./wwwroot/lib/font-awesome'));

    gulp.src('./node_modules/es6-shim/**/*.js')
        .pipe(gulp.dest('./wwwroot/lib/es6-shim'));

    gulp.src('./node_modules/zone.js/**/*.js')
        .pipe(gulp.dest('./wwwroot/lib/zone.js'));

    gulp.src('./node_modules/reflect-metadata/**/*.js')
        .pipe(gulp.dest('./wwwroot/lib/reflect-metadata'));

    gulp.src('./node_modules/systemjs/**/*.js')
        .pipe(gulp.dest('./wwwroot/lib/systemjs'));

    gulp.src('./node_modules/rxjs/**/*.js')
        .pipe(gulp.dest('./wwwroot/lib/rxjs'));

    gulp.src('./node_modules/jquery/**/*.js')
        .pipe(gulp.dest('./wwwroot/lib/jquery'));

    gulp.src('./vendor/jquery.signalR/*.js')
        .pipe(gulp.dest('./wwwroot/lib/jquery.signalR'));

    gulp.src('./node_modules/@angular/**/*.js')
        .pipe(gulp.dest('./wwwroot/lib/@angular'));

    gulp.src('./bower_components/**/*')
        .pipe(gulp.dest('./wwwroot/lib/bower_components'));

    gulp.src('./node_modules/@vaadin/**/*')
    .pipe(gulp.dest('./wwwroot/lib/@vaadin'));

    gulp.src('./node_modules/css-animator/**/*.js')
        .pipe(gulp.dest('./wwwroot/lib/css-animator'));
});

