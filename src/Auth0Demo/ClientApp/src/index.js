import "bootstrap/dist/css/bootstrap.css";
import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import App from "./App";
import { Auth0Provider } from "@auth0/auth0-react";
import registerServiceWorker from "./registerServiceWorker";

const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href");
const rootElement = document.getElementById("root");

ReactDOM.render(
  <BrowserRouter basename={baseUrl}>
    <Auth0Provider
      domain="uveta-demo-auth0.eu.auth0.com"
      clientId="HvZbHkgYg17ZyU4bZ8KgYVC4i7tChnP9"
      redirectUri={window.location.origin}
      audience="https://demo/api"
      scope="openid profile email orders:read orders:full"
    >
      <App />
    </Auth0Provider>
  </BrowserRouter>,
  rootElement
);

registerServiceWorker();
