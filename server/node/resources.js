var azure = require('azure');
var qs = require('querystring');

exports.post = function(request, response) {
    var accountName = 'storage account name goes here'; 
    var accountKey = 'storage account key goes here';
   
    validateInputs(request, response); 

    var expiryDate = formatDate(Date.parse(request.body.expiry));

    var host;
    var resourceType;
    if(request.query.type == 'blob') {
        resourceType = azure.Constants.BlobConstants.ResourceTypes.BLOB;
        host = accountName + '.blob.core.windows.net';
    } else if (request.query.type == 'table') {
        resourceType = azure.Constants.BlobConstants.ResourceTypes.TABLE;
        host = accountName + '.table.core.windows.net';
    }

    var sharedAccessPolicy = {AccessPolicy: { Permissions: "", Expiry: expiryDate }};
    if(request.body.permissions == 'r') {
        sharedAccessPolicy.AccessPolicy.Permissions = azure.Constants.BlobConstants.SharedAccessPermissions.READ
    } else if (request.body.permissions == 'w') {
        sharedAccessPolicy.AccessPolicy.Permissions = azure.Constants.BlobConstants.SharedAccessPermissions.WRITE
    } else if (request.body.permissions == 'rw' || request.body.permissions == 'wr') {
        sharedAccessPolicy.AccessPolicy.Permissions = azure.Constants.BlobConstants.SharedAccessPermissions.READ + azure.Constants.BlobConstants.SharedAccessPermissions.WRITE;
    }

    var relativePath = '/' + request.body.container + '/' + request.body.name;
    
    // Form the SAS token
    var resourceUrlSAS = createResourceURLWithSAS(accountName, accountKey, resourceType, relativePath, sharedAccessPolicy, host);
    
    response.send(statusCodes.OK, { "uri" : resourceUrlSAS });
};

function createResourceURLWithSAS(accountName, accountKey, resourceType, blobRelativePath, sharedAccessPolicy, host) {
    // Generate the SAS for blob
    var sasQueryString = getSAS(accountName,
            accountKey,
            blobRelativePath,
            resourceType,
            sharedAccessPolicy);

    // Full path for resource with SAS
    return  'https://' + host + blobRelativePath + '?' + sasQueryString;
} 

function getSAS(accountName, accountKey, path, resourceType, sharedAccessPolicy) { 
    console.log("name:" + accountName + " key:" + accountKey + " path:" + path + " type:" + resourceType + " access:" + sharedAccessPolicy.AccessPolicy );
    var sas = new azure.SharedAccessSignature(accountName, accountKey);
    return qs.encode(sas.generateSignedQueryString(
                    path, 
                    {}, 
                    resourceType, 
                    sharedAccessPolicy)); 
} 


function validateInputs(request, response)
{
    console.log("request:" + request + " response:" + response);
    // Validate existence of query parameters
    if(request.query.type == null)
    {
        response.send(400, "type parameter is null");
    }
    
    if( !(request.query.type == "table" || request.query.type == "blob"))
    {
        response.send(400, "type parameter is invalid");
    }
    
    // Validate existence of body parameters
    if(request.body.name == null 
            || request.body.permissions == null 
            || request.body.expiry == null 
            || request.body.container == null)
    {
        response.send(400, "parameters are incomplete");
    }


    // Validate blob name 
    if(request.body.name == "")
    {
        request.send(400, "name is invalid");
    }

    // Validate permissions string
    if(!(request.body.permissions == "r" 
                || request.body.permissions == "w" 
                || request.body.permissions == "rw"
                || request.body.permissions == "wr"))
    {
        response.send(400, "permissions is invalid value");
    }

    // Validate expiry
    var expiryDate = new Date(Date.parse(request.body.expiry));
    if( Object.prototype.toString.call(expiryDate) != "[object Date]" 
            || isNaN(expiryDate.getTime()))
    {
        response.send(400, "expiry is invalid Date object");
    }

    // Validate container
    if(request.body.container == "")
    {
        response.send(400, "invalid container name");
    }
}

function formatDate(date) { 
    var raw = date.toJSON(); 
    // Blob service does not like milliseconds on the end of the time so strip 
    return raw.substr(0, raw.lastIndexOf('.')) + 'Z'; 
}

