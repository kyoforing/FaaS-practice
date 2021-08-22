const { google } = require('googleapis');
const kgsearch = google.kgsearch('v1');

exports.kgSearch = async (req, res) => {
	console.log(req.body);
	const result = await makeSearchRequest(req.body.text);

	res.status(200).send(result);
};

const makeSearchRequest = async query => {
	return new Promise((resolve, reject) => {
		kgsearch.entities.search({
			auth: "xxxx",
			query,
			limit: 1
		}, (err, response) => {
			console.log(err);
			if (err) {
				reject(err);
			}
			resolve(formatSlackMessage(query, response));
		});
	});
}

const formatSlackMessage = (query, response) => {
	let entity;

	if (response && response.data && response.data.itemListElement && response.data.itemListElement.length > 0) {
		entity = response.data.itemListElement[0].result;
	}

	const slackMessage = {
		response_type: 'in_channel',
		text: `Query: ${query}`,
		attachments: []
	};

	if (entity) {
		const attachment = {
			color: '#3367d6'
		};
		if (entity.name) {
			attachment.title = entity.name;
			if (entity.description) {
				attachment.title = `${attachment.title}: ${entity.description}`;
			}
		}

		if (entity.detailedDescription) {
			if (entity.detailedDescription.url) {
				attachment.title_link = entity.detailedDescription.url;
			}
			if (entity.detailedDescription.articleBody) {
				attachment.text = entity.detailedDescription.articleBody;
			}
		}

		if (entity.image && entity.image.contentUrl) {
			attachment.image_url = entity.image.contentUrl;
		}
		slackMessage.attachments.push(attachment);
	} else {
		slackMessage.attachments.push({
			text: 'No results match your query...'
		});
	}

	return slackMessage;
}