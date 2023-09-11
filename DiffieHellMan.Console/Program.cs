// See https://aka.ms/new-console-template for more information

using System.Threading.Channels;
using DiffieHellMan.Console;

Console.WriteLine("Hello, World!");

var clientPublicKey = DiffieHellmanKeyExchange.CreatePublicKey();

var serverPublicKey = DiffieHellmanKeyExchange.CreatePublicKey();

// Client and Server shares public Keys and use private keys  to create sharedKey

var clientShared  = DiffieHellmanKeyExchange.CreateSharedKey(serverPublicKey.PublicKeyBytes, clientPublicKey.PrivateKey);

var serverSharedKey  = DiffieHellmanKeyExchange.CreateSharedKey(clientPublicKey.PublicKeyBytes, serverPublicKey.PrivateKey);


const string TextToEncrypt = "Encrypt ME!!!";

//The client encrypts data using AES-GCM. You can use an encryption technique of your preference.
var encryptedText = CryptoHelper.EncryptWithMetaData(TextToEncrypt,clientShared);

//Decrypt Encrypted data
var decryptedData = CryptoHelper.DecryptWithMetaData<string>(encryptedText.encryptedBytes,encryptedText.Nonce,encryptedText.Mac, serverSharedKey);

Console.WriteLine();