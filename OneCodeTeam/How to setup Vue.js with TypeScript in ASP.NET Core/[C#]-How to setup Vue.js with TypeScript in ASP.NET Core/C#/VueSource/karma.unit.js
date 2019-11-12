var webpackConfig = require('./config/webpack.config.test');

module.exports = function(config) {
  config.set({
    basePath: '',
    frameworks: ['mocha', 'chai', 'sinon'],
    files: [
      'node_modules/es6-promise/dist/es6-promise.auto.js',
      'src/test.ts'
    ],
    preprocessors: {
      'src/test.ts': ['webpack']
    },
    webpack: webpackConfig,
    webpackServer: { noInfo: true },
    reporters: ['mocha'],
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: false,
    browsers: ['Chrome'],
    mime: {
      'text/x-typescript': ['ts']
    },
    singleRun: true
  });
};
