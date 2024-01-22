import './assets/main.css'

import Keycloak from 'keycloak-js';

const keycloak = new Keycloak({
	url: 'http://localhost:8080',
	realm: 'aspnet',
	clientId: 'aspnet-api'
});

import { createApp } from 'vue'

import App from './App.vue'


keycloak.init({
	onLoad: "login-required"
}).then(authenticated => {
	if (!authenticated) {
		window.location.reload();
		return;
	}

	createApp(App).mount('#app')

	setInterval(() => {
		keycloak.updateToken(70).success((refreshed) => {
			if (refreshed) {
				consoel.log('Token refreshed' + refreshed);
			} else {
				console.warn('Token not refreshed, valid for ' + Math.round(keycloak.tokenParsed.exp + keycloak.timeSkew - new Date().getTime() / 1000) + ' seconds');
			}
		}).error(() => {
			console.error('Failed to refresh token');
		});


	}, 60000)

	console.log(`User is ${authenticated ? 'authenticated' : 'not authenticated'}`);
});