import React, { Component } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/Home";
import FetchData from "./components/FetchData";
import { Counter } from "./components/Counter";
import ProtectedRoute from "./components/ProtectedRoute";

import "./custom.css";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path="/" component={Home} />
        <ProtectedRoute path="/counter" component={Counter} />
        <ProtectedRoute path="/fetch-data" component={FetchData} />
      </Layout>
    );
  }
}
