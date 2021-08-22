'use strict';

exports.hello = event => {
  console.log(event);
  console.log('Hello World');
  return true;
};