<?php

ini_set('display_errors', true);
error_reporting(E_ALL);

$user=$_GET['user'];
$mineType=$_GET['mineType'];


$isValid = ValidateUser($user);
$userMsg = "Invalid User - Please contact the developer at tmfrisinger@gmail.com";
$isUserMsgError = true;

$userConfig = new MiningUserConfig;
$userConfig->MiningUser = $user;

if($isValid){
	$userMsg = "Login Successful";
	// Fetch balance, rank, pools, keys, etc....
	$balance = FetchBalance($user);
	$rank = FetchRank($user);
	$balanceTS = FetchBalanceTS();
	$isUserMsgError = false;
	// wallet
	$wallet = FetchWalletForMiningType($user, $mineType);
	
	// populate user object
	$userConfig->IsValid = $isValid;
	$userConfig->UserMessage = $userMsg;
	$userConfig->IsUserMessageError = $isUserMsgError;
	$userConfig->MiningWallet = $wallet;
	$userConfig->UserRank = $rank;
	$userConfig->Balance = $balance;
	$userConfig->BalanceTs = $balanceTS;
	
}else{
	$userConfig->IsUserMessageError = $isUserMsgError;
	$userConfig->UserMessage = $userMsg;
	$userConfig->IsValid = $isValid;
}

// json convert and send ;)
header('Content-Type:text/html');
echo json_encode($userConfig);

// -- Start Functions -- 
function FetchWalletForMiningType($user, $mineType){
	$fileName= "dgc_wallet.csv";

	// Profitable Mining ;)
	if(stripos($mineType,"profitable") >= 0){
		$fileName = "btc_wallet.csv";
		LogMessage($user, "BTC Wallet");
	}else{
		LogMessage($user, $mineType." Wallet");
	}
	
	$lines = file($fileName, FILE_IGNORE_NEW_LINES);
	$pos=0;
	$found=0;
	$wallet = array();

	// build balances
	for($x=0; $x < count($lines); $x++){
		$tmp = explode(",",$lines[$x]);
		$wallet[$tmp[0]] = $tmp[1];
	}
	
	$result = "error";
	try{
		$result = $wallet[$user];
	}catch(Exception $e){
	}
	
	return $result;
}

// Log a Message ;)
function LogMessage($user, $msg){
    $fileName= "mining_log.txt";

    date_default_timezone_set("Africa/Harare");
    $ts = date("d-m-Y H:i:s", time());
    file_put_contents($fileName, $ts." :: ".$user." -> ".$msg."\n", FILE_APPEND);
}

// Validate User
function ValidateUser($user){
	$lines = file("mining_users.txt", FILE_IGNORE_NEW_LINES);
	$pos=0;
	$found=0;

	while($found == 0 && $pos < count($lines)){
		if($user == $lines[$pos]){
			$found = 1;
		}
		$pos++;
	}

	if($found == 1){
		return true;
	}else{
		return false;
	}
}

//  Fetch Rank
function FetchRank($user){
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

	// sort and return index and count ;)
	arsort($bal,1);
	$len = count($bal);
	if (array_key_exists($user,$bal))
	{
	   $pos = array_search($user, array_keys($bal));
	   $pos = $pos+1;
	   return $pos." of ".$len;
	}
	else
	{
	  return "0 of 0";
	}
}

// Fetch Balance
function FetchBalance($user){
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
	}
	
	return $result;
}

function FetchBalanceTS(){
	$fileName= "mining_balances.txt";
	# get last modified ts
	date_default_timezone_set("Africa/Harare");
	$ts = date("d-m-Y H:i:s", filemtime($fileName));

	return $ts;
}

class MiningUserConfig{

	public $MiningUser;

	public $IsValid;
	
	public $Balance;

	//public $BalanceCurrency;

	public $BalanceTs;

	public $UserRank;

	//public MiningPool[] MiningPools { get; set; }

	public $MiningWallet;

	public $UserMessage;

	public $IsUserMessageError;
}

?>

