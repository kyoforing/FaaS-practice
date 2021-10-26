const handler = require('../handler');

test('correct en greeting is generated', () => {
  expect(handler.getLocalGreeting("en")).toBe("Hello!");
});

test('correct fr greeting is generated', () => {
  expect(handler.getLocalGreeting("fr")).toBe("👋");
});