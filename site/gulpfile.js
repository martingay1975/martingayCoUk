var gulp = require("gulp");
var sass = require("gulp-sass");
var watch = require("gulp-watch");

var sassConfig = {
	inputDirectory: "Content/main.scss",
	outputDirectory: "Content",
	options: {
		outputStyle: "expanded"
	}
};

var buildcss = function() {
	return gulp
		.src(sassConfig.inputDirectory)
		.pipe(sass(sassConfig.options).on("error", sass.logError))
		.pipe(gulp.dest(sassConfig.outputDirectory));
}

gulp.task("build-css", buildcss);

gulp.task("watch", function () {
	watch("Scripts/**/**/*.scss")
		.on("change", function (file) {
			buildcss();
			console.log(`Compiled Sass ${file}`);
		});
});
