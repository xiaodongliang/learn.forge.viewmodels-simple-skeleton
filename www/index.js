$(document).ready(function () {

    $('#getToken').click(function () {

        jQuery.ajax({
            url: '/api/forge/oauth/token',
            success: function (res) {
                //callback(res.access_token, res.expires_in)
                $('#tokenvalue').val(res.access_token); 
            }, error: function (err) {
                $('#tokenvalue').val(err.responseJSON.ExceptionMessage);
            }
        }); 
    }); 

    $('#createbucket').click(function () {

        var bucketKey = $('#newbucketname').val();
        jQuery.post({
            url: '/api/forge/oss/buckets',
            contentType: 'application/json',
            data: JSON.stringify({ 'bucketKey': bucketKey, 'policyKey': 'persistent' }),
            success: function (res) {
                $('#buckettatus').val('succeded!');
             },
            error: function (err) {
                if (err.status == 409) 
                    $('#buckettatus').val('Bucket already exists - 409: Duplicated'); 
                else
                    $('#buckettatus').val(err.responseJSON.ExceptionMessage);
            }
        });
    }); 

    $('#uploadfile').click(function () {

        $('#hiddenUploadField').click();

        $('#hiddenUploadField').unbind().change(function () {
            if (this.files.length == 0) return;
            var file = this.files[0];
            var formData = new FormData();
            formData.append('fileToUpload', file);
            var bucketKey = $('#newbucketname').val();
            formData.append('bucketKey', bucketKey);

            $.ajax({
                url: '/api/forge/oss/objects',
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (data) {
                    $('#uploadurn').val(data.objectId);
                }, error: function (err) {
                    $('#uploadurn').val(err.responseJSON.ExceptionMessage);
                }
            });
        });
    });


    $('#postJob').click(function () {

        var objectKey = $('#uploadurn').val();

        jQuery.post({
            url: '/api/forge/modelderivative/jobs',
            contentType: 'application/json',
            data: JSON.stringify({ 'objectKey': objectKey }),
            success: function (res) {
                $("#postjobstatus").val('translation started!');

                $("#base64urn").val(res.base64urn);
            }, error: function (err) {
                $('#postjobstatus').val(err);
            }
        });
    }); 

    $('#getjobsprogress').click(function () {

        var objectKey = $('#base64urn').val();

        jQuery.get({
            url: '/api/forge/modelderivative/status' + '?'+ 'objectKey='+objectKey,
            contentType: 'application/json',
            success: function (res) {
                $("#jobsprogresss").val(res.progress);
            }, error: function (err) {
                $('#jobsprogresss').val(err.responseJSON.ExceptionMessage);
            }
        });
    }); 

    $('#loadviewer').click(function () {
        var objectKey = $('#base64urn').val();

        launchViewer(objectKey);
    }); 

});
 