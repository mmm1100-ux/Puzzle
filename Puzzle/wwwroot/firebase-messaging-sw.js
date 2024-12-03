
// Service Worker
importScripts('/lib/firebase-app.js');
importScripts('/lib/firebase-messaging.js');

const firebaseConfig = {
    apiKey: "AIzaSyDJlKZz5DB-Ux2nYg80q8V-wIIlpQ7f_DI",
    authDomain: "puzzle-13d5e.firebaseapp.com",
    projectId: "puzzle-13d5e",
    storageBucket: "puzzle-13d5e.appspot.com",
    messagingSenderId: "58308022725",
    appId: "1:58308022725:web:a40123a148fdde0edbfbac",
    measurementId: "G-6LVSM5GLR4"
};

// // Initialize Firebase
const app = firebase.initializeApp(firebaseConfig);

// Initialize Firebase Cloud Messaging and get a reference to the service
const messaging = firebase.messaging()

messaging.onBackgroundMessage(payload => {

    console.log(payload);

    const notificationOptions = {
        body: payload.notification.body,
        icon: '/images/icons/192.png'
    };

    self.registration.showNotification(payload.notification.title,
        notificationOptions);
});