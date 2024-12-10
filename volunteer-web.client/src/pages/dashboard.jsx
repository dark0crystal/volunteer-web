import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"; // Import the useNavigate hook
import UserDashboard from "../components/dashboard/Dashboard";
import VolunteerDashboard from "../components/dashboard/VolunteerDashboard";

export default function Dashboard() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const navigate = useNavigate(); // Hook for navigation

    useEffect(() => {
        const checkSession = async () => {
            try {
                const response = await fetch("https://localhost:7149/Users/check-session", {
                    method: "GET",
                    credentials: "include", 
                });

                if (response.ok) {
                    // User is logged in, proceed to show the dashboard
                    const data = await response.json();
                    console.log("Logged in as: ", data.userEmail);

                    setIsLoggedIn(true);
                } else {
                    // User is not logged in, redirect to login page
                    navigate("/login");
                }
            } catch (error) {
                console.error("Error checking session: ", error);
                // Redirect to login page in case of error
                navigate("/login");
            }
        };

        checkSession();
    }, [navigate]); // Re-run the effect if the navigate hook changes

    if (!isLoggedIn) {
        return null; // Return nothing while redirecting
    }

    return (
        <div>
            <VolunteerDashboard />
            <UserDashboard />
        </div>
    );
}

