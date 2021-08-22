'use strict';

module.exports.hello = async (event) => {
  console.log(event);
  console.log('Hello World');
  return true;
};
