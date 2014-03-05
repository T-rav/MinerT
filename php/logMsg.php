<?php

ini_set('display_errors', true);
error_reporting(E_ALL);

$user=$_GET['user'];
$msg=$_GET['msg'];
$fileName= "mining_log.txt";

date_default_timezone_set("Africa/Harare");
$ts = date("d-m-Y H:i:s", time());
file_put_contents($fileName, $ts." :: ".$user." -> ".$msg."\n", FILE_APPEND);

header('Content-Type:text/html');
echo "ok";

?>

