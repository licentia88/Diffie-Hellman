# Diffie-Hellman

This repository demonstrates how to implement Diffie–Hellman Key Exchange using Bouncy castle!
It appears that the ECDiffieHellmanCng class is primarily designed for Windows environment, and does not support mono. 
I hope this documentation will assist anyone who encounters difficulties while working with Diffie-Hellman.


##### ECDiffieHellmanCng Class 
provides a Cryptography Next Generation (CNG) implementation of the Elliptic Curve Diffie-Hellman (ECDH) algorithm. This class is used to perform cryptographic operations. A basic example could be as follows:

```csharp
ECDiffieHellmanCng alice = new ECDiffieHellmanCng();

alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
alice.HashAlgorithm = CngAlgorithm.Sha256;

ECDiffieHellmanCng bob = new ECDiffieHellmanCng();
bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
bob.HashAlgorithm = CngAlgorithm.Sha256;

byte[] bobSharedKey = bob.DeriveKeyMaterial(alice.PublicKey);
byte[] aliceSharedKey = alice.DeriveKeyMaterial(bob.PublicKey);

```
It's super easy! but the problem here is that 
ECDiffieHellmanCng is not available on mono either on linux or macos but worry not! Bouncy Castle is here for the rescue!


### Using Bouncy Castle

```csharp
public static class DiffieHellmanKeyExchange
{
    /// <summary>
    /// Generates a new ECDH public key and returns the raw bytes.
    /// </summary>
    /// <returns>The public key as a byte array</returns>
    public static (byte[] PublicKeyBytes, AsymmetricKeyParameter PrivateKey) CreatePublicKey()
    {
        // Create EC domain parameters using named curve
        X9ECParameters ecP = ECNamedCurveTable.GetByName("P-256");
        ECDomainParameters ecParams = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H);

        // Generate new EC key pair 
        SecureRandom random = new SecureRandom();
        ECKeyPairGenerator keyGen = new ECKeyPairGenerator();
        keyGen.Init(new ECKeyGenerationParameters(ecParams, random));
        AsymmetricCipherKeyPair serverKeyPair = keyGen.GenerateKeyPair();

        // Extract public key from pair
        ECPublicKeyParameters serverPubKey = serverKeyPair.Public as ECPublicKeyParameters;

        // Encode public key to raw bytes
        byte[] publicKeyBytes = serverPubKey.Q.GetEncoded();

        // Return the raw public key bytes
        return (publicKeyBytes, serverKeyPair.Private);
    }

    /// <summary>
    /// Computes ECDH shared secret using the received public key.
    /// </summary>
    /// <param name="publicKeyBytes">The public key from server</param>
    /// <returns>Shared secret as byte array</returns>
    public static byte[] CreateSharedKey(byte[] publicKeyBytes, AsymmetricKeyParameter privateKey)
    {
        X9ECParameters ecP = ECNamedCurveTable.GetByName("P-256");
        ECDomainParameters ecParams = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H);

        // Convert received public key bytes to an ECPublicKeyParameters
        ECPublicKeyParameters receivedPublicKey = new ECPublicKeyParameters(
            ecParams.Curve.DecodePoint(publicKeyBytes), ecParams);

        // Perform key agreement
        ECDHBasicAgreement agreement = new ECDHBasicAgreement();
        agreement.Init(privateKey);
        BigInteger sharedSecret = agreement.CalculateAgreement(receivedPublicKey);

        // Convert shared secret to byte array
        byte[] sharedSecretBytes = sharedSecret.ToByteArrayUnsigned();

        return sharedSecretBytes;
    }
}
```

### Sample Usage using AES-GCM for encrypt/decrypt :

```csharp
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

```

Download DiffieHellMan.Console and then run the app to perform a test.


