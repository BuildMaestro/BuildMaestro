/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

'use strict'

// 
var ext_replace = require('gulp-ext-replace');
var gulp = require('gulp');
var rollup = require('rollup');
var typescript = require('rollup-plugin-typescript');
var uglify = require('gulp-uglifyjs');

gulp.task('default', function () {
    // place code for your default task here
});

gulp.task('build', function () {
    process.stdout.write('Starting rollup.\n');

    //// Custom Rollup Plugin to resolve rxjs deps
    //// Thanks to https://github.com/IgorMinar/new-world-test/blob/master/es6-or-ts-bundle/rollup.config.js
    //class RollupNG2 {
    //    constructor(options) {
    //        this.options = options;
    //    }
    //    resolveId(id, from) {
    //        if (id.startsWith('rxjs/')) {
    //            return `${__dirname}/node_modules/rxjs-es/${id.replace('rxjs/', '')}.js`;
    //        }
    //    }
    //}
    //const rollupNG2 = (config) => new RollupNG2(config);

    //rollup.rollup({
    //    entry: 'wwwroot/app/main.ts',
    //    format: 'cjs',
    //    plugins: [
    //      typescript(),
    //      rollupNG2()
    //    ]
    //}).then(function (bundle) {
    //    bundle.write({
    //        format: 'cjs',
    //        dest: './wwwroot/app/buildmaestro.js'
    //    });
    //});

    //process.stdout.write('Finished rollup.\n');

    //process.stdout.write('Starting uglify.\n');

    //gulp.src('./wwwroot/app/buildmaestro.js')
    //    .pipe(uglify())
    //    .pipe(ext_replace('.min.js'))
    //    .pipe(gulp.dest('./wwwroot/app'));

    //process.stdout.write('Finished uglify.\n');
})

