import { useParams } from "react-router-dom";
import { useState, useEffect } from 'react';

export default function PostDetails() {
    const { postId } = useParams();
    const [details, setDetails] = useState(null);
    const [error, setError] = useState(null);
    const [userEmail, setUserEmail] = useState(''); // State to hold user email
    const [successMessage, setSuccessMessage] = useState(''); // State for success message

    useEffect(() => {
        async function getDetail() {
            try {
                const res = await fetch(`https://localhost:7149/Posts/post-details`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(parseInt(postId)), // Convert postId to integer
                });

                if (!res.ok) {
                    throw new Error("Failed to fetch post details");
                }

                const data = await res.json();
                setDetails(data);
            } catch (err) {
                setError(err.message);
            }
        }

        getDetail();
    }, [postId]);

    const handleSavePost = async () => {
        try {
            const res = await fetch('https://localhost:7149/Posts/save-post', {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ email: userEmail, postId: parseInt(postId) }), // Send email and postId
            });

            if (!res.ok) {
                throw new Error("Failed to save post");
            }

            const message = await res.text();
            setSuccessMessage(message); // Set success message
        } catch (err) {
            setError(err.message);
        }
    };

    if (error) {
        return <div className="alert alert-danger" role="alert">{error}</div>;
    }

    if (!details) {
        return <div className="text-center">Loading...</div>;
    }

    return (
        <div className="container mt-5">
            <div className="card">
                <div className="card-header">
                    <h1 className="card-title">{details.title}</h1>
                </div>
                <div className="card-body">
                    <h4 className="card-subtitle mb-2 text-muted">{details.orgName}</h4>
                    <p className="card-text">{details.description}</p>
                    <p className="card-text"><strong>Location:</strong> {details.location}</p>
                    <p className="card-text"><strong>Start Date:</strong> {details.startDate}</p>
                    <p className="card-text"><strong>End Date:</strong> {details.endDate}</p>
                    <p className="card-text"><strong>Number of Days:</strong> {details.numOfDays}</p>

                    {/* Input for user email */}
                    <div className="mb-3">
                        <label htmlFor="userEmail" className="form-label">Your Email:</label>
                        <input
                            type="email"
                            className="form-control"
                            id="userEmail"
                            value={userEmail}
                            onChange={(e) => setUserEmail(e.target.value)}
                            required
                        />
                    </div>

                    {/* Button to save post */}
                    <button className="btn btn-primary" onClick={handleSavePost}>
                        Save Post
                    </button>

                    {/* Display success message */}
                    {successMessage && <div className="alert alert-success mt-3">{successMessage}</div>}
                </div>
                <div className="card-footer text-muted">
                    Posted by: {details.postAdminEmail}
                </div>
            </div>
        </div>
    );
}
