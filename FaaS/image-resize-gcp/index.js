const Sharp = require('sharp');
const storage = require('@google-cloud/storage')();

exports.resizeImage = async event => {
    console.log(event);

    const sourceBucket = event.bucket;
    const sourceFileKey = event.name;
    const destinationBucket = `${sourceBucket}-resize`
    const resizeConfigs = getResizeConfigs(sourceFileKey);
    const tempLocalPath = `/tmp/${sourceFileKey}`;

    ensureTypeCorrect(sourceFileKey);

    await downloadResource(sourceBucket, sourceFileKey, tempLocalPath);

    for (let resizeConfig of resizeConfigs) {
        await Sharp(tempLocalPath, { failOnError: false })
            .resize(resizeConfig.width)
            .toFile(`/tmp/${resizeConfig.resizeFileKey}`);
    
        await storage
            .bucket(destinationBucket)
            .upload(`/tmp/${resizeConfig.resizeFileKey}`, {destination: resizeConfig.resizeFileKey});
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

const downloadResource = async (bucket, fileKey, tempLocalPath) => {
    await storage
        .bucket(bucket)
        .file(fileKey)
        .download({ destination: tempLocalPath });
};