<?php 
try{
   $returnValue = TRUE;
    $accToken = $_COOKIE["moss_access_token"];

    $url = $_REQUEST["target"] . "/_api/web/GetFolderByServerRelativeUrl('Lists/SharedDoc')/Files/add(url='" . $_FILES["file"]["name"] . "',overwrite=true)";
 
    $furl = $_FILES["file"]["tmp_name"];
    $file = fopen($furl,"r");
    $post_data = fread($file,filesize($furl));
    fclose($file);

    $opts = array (
        'X-RequestDigest:' . $_REQUEST["digest"],
        'Authorization: Bearer ' . $accToken
	);
    // Initialize cURL
    $ch = curl_init();
	curl_setopt($ch, CURLOPT_HTTPHEADER, $opts);
    // Set URL on which you want to post the Form and/or data
    curl_setopt($ch, CURLOPT_URL, $url);
    // Data+Files to be posted
	curl_setopt($ch, CURLOPT_POST, 1);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $post_data);
    // Pass TRUE or 1 if you want to wait for and catch the response against the request made
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
    // For Debug mode; shows up any error encountered during the operation
    curl_setopt($ch, CURLOPT_VERBOSE, 1);
	curl_setopt($ch, CURLOPT_HEADER, 0);
	curl_setopt ($ch, CURLOPT_SSL_VERIFYHOST, 0);
    curl_setopt ($ch, CURLOPT_SSL_VERIFYPEER, 0);

    // Execute the request
    $response = curl_exec($ch);
    curl_close($ch); 

}
catch(Exception $ex)
{
   echo $ex->getMessage();
}
?> 

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <title>upload_file</title>
</head>
<body>
    <?php if ($returnValue)
            {
                echo '<script>';
                echo 'window.onload = function () { window.parent.window.location = window.parent.window.location;}';
                echo '</script>';
            }
            else{
                $errorMsg;
            }
    ?>
</body>
</html>