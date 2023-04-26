const functions = require("firebase-functions");
const admin = require("firebase-admin");
const path = require("path");

// // Create and deploy your first functions
// // https://firebase.google.com/docs/functions/get-started
//
// exports.helloWorld = functions.https.onRequest((request, response) => {
//   functions.logger.info("Hello logs!", {structuredData: true});
//   response.send("Hello from Firebase!");
// });

admin.initializeApp();


exports.saveFilename = functions.storage.object().onFinalize(async (object) => {
  const fileBucket = object.bucket; // The Storage bucket that contains the file
  const filePath = object.name; // File path in the bucket.
  const contentType = object.contentType; // File content type.

  const fileName = path.basename(filePath);
  const groupName = path.dirname(filePath);

  admin.firestore().collection("files").doc(groupName).collection("files").add({
    name: fileName,
    contentType: contentType,
    bucket: fileBucket,
  });
});


exports.files = functions.https.onRequest((req, res) => {
  const files = [];
  admin.firestore()
      .collection("files")
      .doc(req.query.group)
      .collection("files")
      .get().then((snapshot) => {
        snapshot.docs.forEach((doc) =>
          files.push({filename: doc.data().name}));
        res.send(files);
        return "";
      }).catch((reason) =>
        res.send(reason),
      );
});
