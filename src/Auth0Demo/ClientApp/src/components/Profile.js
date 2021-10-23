import React from "react";
import { useAuth0 } from "@auth0/auth0-react";
import LogoutButton from "./LogoutButton";
import LoginButton from "./LoginButton";
import "./Profile.css";

const Profile = () => {
  const { user, isAuthenticated, isLoading } = useAuth0();

  if (isLoading) {
    return <div>Loading ...</div>;
  }

  if (isAuthenticated) {
    return (
      <span className="login">
        {/* <img src={user.picture} alt={user.name} /> */}
        {/* <h2>{user.name}</h2> */}
        <span>{user.email}</span>
        <LogoutButton />
      </span>
    );
  }

  return (
    <span className="login">
      <LoginButton />
    </span>
  );
};

export default Profile;
