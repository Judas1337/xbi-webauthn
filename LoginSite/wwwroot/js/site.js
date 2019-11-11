// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

jQuery(document).ready(function () {

    $('#makeCredential').click(async function () {
        console.log("makeCredential");

        const credential = await navigator.credentials.create({
            publicKey: publicKeyCredentialCreationOptions
        });


        console.log(credential);
        var clientJson = String.fromCharCode.apply(null, new Uint8Array(credential.response.clientDataJSON));
        var attestation = String.fromCharCode.apply(null, new Uint8Array(credential.response.attestationObject));
        //console.log(clientJson, attestation);    
        //$.ajax({
        //    type: "GET",
        //    url: "https://localhost:44306/api/Credential/kalle",
        //    contentType: "application/json"
        //});

        console.log(credential.id);
        var json = publicKeyCredentialToJSON(credential);

        console.log(json);
        $.ajax({
            type: "POST",
            data: JSON.stringify(credential),
            url: "https://localhost:44319/credentials",
            contentType: "application/json"
        });

        //$.ajax({
        //    type: "PUT",
        //    data: JSON.stringify(credential),
        //    url: "https://localhost:44306/api/Credential",
        //    contentType: "application/json"
        //});

    });

    $('#login').click(async function () {

        const cred = await navigator.credentials.get({
            publicKey: publicKeyCredentialRequestOptions
        });
        console.log(cred);
    });

});

async function register() {

    console.log("register");
    const credential = await navigator.credentials.create({
        publicKey: publicKeyCredentialCreationOptions
    });
    
    console.log(credential);

    const decodedAttestationObj = CBOR.decode(
        credential.response.attestationObject);

    console.log(decodedAttestationObj);

    var json = publicKeyCredentialToJSON(credential);

    console.log(json);
    $.ajax({
        type: "POST",
        data: JSON.stringify(json),
        url: "https://localhost:44319/credentials/register",
        contentType: "application/json"
    });
}

const publicKeyCredentialCreationOptions = {

    challenge: Uint8Array.from(
        "xlent", c => c.charCodeAt(0)),
    rp: {
        name: "Duo Security",
        id: "localhost"
    },
    user: {
        id: Uint8Array.from(
            "UZSL85T9AFC", c => c.charCodeAt(0)),
        name: "lee@webauthn.guide",
        displayName: "Lee"
    },
    pubKeyCredParams: [{ alg: -7, type: "public-key" }],
    authenticatorSelection: {
        authenticatorAttachment: "cross-platform",
    },
    timeout: 60000,
    attestation: "direct"
};

const publicKeyCredentialRequestOptions = {
    challenge: Uint8Array.from(
        "xlent", c => c.charCodeAt(0)),
    allowCredentials: [{
        id: Uint8Array.from(
            "UZSL85T9AFC", c => c.charCodeAt(0)),
        type: 'public-key',
        transports: ['usb', 'ble', 'nfc'],
    }],
    timeout: 60000
};

const publicKeyCredentialToJSON = (
    pubKeyCred
) => {
        if (pubKeyCred instanceof Array) {
            let arr = [];
            for (let i of pubKeyCred)
                arr.push(publicKeyCredentialToJSON(i));

            return arr;
        }

        if (pubKeyCred instanceof ArrayBuffer) {
            return base64url.encode(pubKeyCred);
        }

        if (pubKeyCred instanceof Object) {
            let obj = {};

            for (let key in pubKeyCred) {
                obj[key] = publicKeyCredentialToJSON(pubKeyCred[key])
            }

            return obj;
        }

        return pubKeyCred;
    };