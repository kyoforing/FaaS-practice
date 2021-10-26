'use strict';

module.exports.endpoint = async () => {
  const response = {
    statusCode: 200,
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      message: getLocalGreeting(pickLocale()),
    }),
  };

  return response;
};

const pickLocale = () => {
  const languages = ["en", "es", "cn", "fr", "ru"];
  return languages [Math.floor(Math.random() * languages.length)];
}

const getLocalGreeting = language => {
  switch(language) {
    case "en":
      return "Hello!";
    case "es":
      return "¡Hola!";
    case "ru":
      return "Привет!";
    default:
      return "👋";
  }
}

module.exports.getLocalGreeting = getLocalGreeting;