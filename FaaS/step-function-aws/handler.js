'use strict';
const AWS = require('aws-sdk');
const stepfunctions = new AWS.StepFunctions();

module.exports.firstStep = async event => {
  const params = {
    stateMachineArn: process.env.STEP_FUNCTION_ARN,
    input: {}
  };

  await stepfunctions.startExecution(params);

  console.log('first step')
  console.log(event);

  return event;
};

module.exports.endStep = async event => {
  console.log('end step')
  console.log(event);

  return event;
};

module.exports.choiceStep = async event => {
  console.log('choice step')
  event.randomNumber = Math.floor(Math.random() * 2);
  console.log(event);

  return event;
};