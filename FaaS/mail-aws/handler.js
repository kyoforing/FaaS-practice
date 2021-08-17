'use strict';
const { WebClient } = require('@slack/web-api');
const token = "xxxx";

module.exports.sesEvent = async (event) => {
  console.log(JSON.stringify(event));

  try {
    const message = JSON.parse(event.Records[0].Sns.Message);
    const mailType = message.notificationType;
    const receiver = message.mail.destination.join(', ');
    console.log(mailType);

    if (mailType == 'Bounce') {
      const bounceSummary = message.bounce.bouncedRecipients;
      const invalidReceiver = bounceSummary.filter(el => el.action === 'failed').map(el => el.emailAddress);  

      console.log(`Send mail to ${receiver}, some error occured`);
      console.log(`Bounce mail: ${invalidReceiver.join(', ')}`);

      await sendSlackMessage(`Bounce mail: ${invalidReceiver.join(', ')}`);
    } else if (mailType == 'Delivery') {
      await sendSlackMessage(`Send mail to ${receiver} successfully`);
    }
  } catch (ex) {
    console.log(ex);
  }
};

const sendSlackMessage = async message => {
  const web = new WebClient(token);
  await web.chat.postMessage({
    text: message,
    channel: "xxx",
    as_user: true
  });
}