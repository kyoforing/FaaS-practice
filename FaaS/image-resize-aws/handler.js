// dependencies
const AWS = require('aws-sdk');
const Sharp = require('sharp');
const S3 = new AWS.S3();

module.exports.resizeImage = async event => {
    console.log(JSON.stringify(event));

    const sourceBucket = event.Records[0].s3.bucket.name;
    const sourceFileKey = decodeURIComponent(event.Records[0].s3.object.key.replace(/\+/g, " "));
    const destinationBucket = `${sourceBucket}-resize`
    const resizeConfigs = getResizeConfigs(sourceFileKey);

    ensureTypeCorrect(sourceFileKey);

    const originImage = await getResource(sourceBucket, sourceFileKey);

    for (let resizeConfig of resizeConfigs) {
        const resizeImage = await Sharp(originImage.Body, { failOnError: false })
            .resize(resizeConfig.width)
            .toBuffer();
    
        await S3.putObject({
            Body: resizeImage,
            Bucket: destinationBucket,
            ContentType: resizeImage.ContentType,
            Key: resizeConfig.resizeFileKey
        }).promise();
    }
};

const getResizeConfigs = sourceFileKey => {
    const extention = sourceFileKey.match(/\.([^.]*)$/);

    return [{
        width: 230,
        resizeFileKey: `${sourceFileKey.replace(extention[0], '')}_1x${extention[0]}`
    }, {
        width: 360,
        resizeFileKey: `${sourceFileKey.replace(extention[0], '')}_2x${extention[0]}`
    }, {
        width: 1200,
        resizeFileKey: `${sourceFileKey.replace(extention[0], '')}_3x${extention[0]}`
    }]
};

const ensureTypeCorrect = sourceFileKey => {
    const typeMatch = sourceFileKey.match(/\.([^.]*)$/);
    if (!typeMatch) {
        console.error(`unable to infer image type for file ${sourceFileKey}`);
        return;
    }

    const imageType = typeMatch.length > 1 ? typeMatch[1].toLowerCase() : '';
    if (!['jpg', 'jpeg', 'gif', 'png', 'eps'].includes(imageType)) {
        console.log(`skipping non-image ${sourceFileKey}`);
        return;
    } 
};

const getResource = async (bucket, fileKey) => {
    return await S3.getObject({ 
        Bucket: bucket, 
        Key: fileKey  
    }).promise();
};