import { useState, useEffect } from "react";

export default function UserDashboard() {
    const [credentials, setCredentials] = useState([]);
    const email = "user@gmail.com"; // The email of the user to check.

    useEffect(() => {
        const getCredentials = async () => {
            try {
                const res = await fetch("https://localhost:7149/Posts/dashboard", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(email),
                });

                if (!res.ok) {
                    throw new Error("Failed to fetch posts");
                }

                const data = await res.json();
                console.log(data)
                setCredentials(data);
            } catch (error) {
                console.error("Error fetching credentials:", error);
            }
        };

        getCredentials();
    }, []);

    return (
        <div>
            <h1>User's Posts</h1>
            {credentials.length > 0 ? (
                credentials.map(post => (
                    <div key={post.id}>
                        <h2>{post.description}</h2>
                        <p>{post.location}</p>
                    </div>
                ))
            ) : (
                <p>No posts found. or you need to login</p>
            )}
        </div>
    );
}