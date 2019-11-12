var parseArgs = require('minimist');
var webpackConfig = require('./config/webpack.config.coverage');

var args = parseArgs(process.argv.slice(2), {
  string: ['env'],
  default: {
    'env': 'mocha'
  }
});

var reporters = ['mocha', 'coverage'];

if (args.env === 'tc') {
  reporters = ['teamcity', 'coverage'];
}

if (args.env === 'jk') {
  reporters = ['junit', 'coverage'];
}

module.exports = function (config) {
  config.set({
    basePath: '',
    frameworks: ['mocha', 'chai', 'sinon'],
    files: [
      'node_modules/es6-promise/dist/es6-promise.auto.js',
      'src/test.ts'
    ],
    reporters: reporters,
    preprocessors: {
      'src/test.ts': ['webpack']
    },
    webpack: {
      devtool: 'inline-source-map',
      resolve: webpackConfig.resolve,
      module: webpackConfig.module
    },
    webpackServer: {
      noInfo: true
    },
    junitReporter: {
      outputDir: 'reports/'
    },
    coverageReporter: {
      reporters: [{
        type: 'json',
        dir: 'coverage/json',
        subdir: '.'
      }]
    },
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: false,
    browsers: ['PhantomJS'],
    singleRun: true,
    concurrency: Infinity
  });
};
