<?php

use Psr\Http\Message\ServerRequestInterface;

function get_encrypt_code(ServerRequestInterface $request)
{
    $data = json_decode($request->getBody(), true);

    $simple_string = $data['Payload'];
    $ciphering = 'AES-128-CTR';
    $options = 0;
    $encryption_iv = 'I am iv';
    $encryption_key = 'I am key';

    $encryption = openssl_encrypt($simple_string, $ciphering, $encryption_key, $options, $encryption_iv);
    $response['result']=$encryption;
    header('Content-Type: application/json; charset=utf-8');
    return json_encode($response);
}