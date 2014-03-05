<?php

ini_set('display_errors', true);
error_reporting(E_ALL);

$user=$_GET['user'];
$time = time();

header('Content-Type:text/html');
$fileName= "mining_balances.txt";

$lines = file($fileName, FILE_IGNORE_NEW_LINES);
$pos=0;
$found=0;
$bal = array();

// build balances
for($x=0; $x < count($lines); $x++){
    $tmp = explode(",",$lines[$x]);
    $bal[$tmp[0]] = $tmp[1];
}

$result = 0.0;
try{
	$result = $bal[$user];
}catch(Exception $e){
   $result = "Invalid User";
}

# get last modified ts
date_default_timezone_set("Africa/Harare");
$ts = date("d-m-Y H:i:s", filemtime($fileName));

echo $result;

?>

