import { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";

export default function DisplayComplaints({ postId }) {
    const [complaints, setComplaints] = useState([]);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (!postId) return;

        const fetchComplaints = async () => {
            try {
                const response = await fetch(`https://localhost:7149/Complaints/GetComplaintsByPost?postId=${postId}`);
                if (response.ok) {
                    const data = await response.json();
                   
                    setComplaints(data);
                    console.log(complaints)
                } else {
                    const errorData = await response.json();
                    throw new Error(errorData.message || "Failed to fetch complaints.");
                }
            } catch (err) {
                setError(err.message);
            }
        };

        fetchComplaints();
    }, [postId]);

    if (error) {
        return (
            <div className="alert alert-danger">
                <strong>Error: </strong> {error}
            </div>
        );
    }
    console.log(complaints)

    return (
        <div className="mt-3">
            <h6>Complaints:</h6>
            {complaints.length > 0 ? (
                <ul className="list-group">
                    {complaints.map((complaint) => (
                        <li key={complaint.id} className="list-group-item">
                            <strong>Type:</strong> {complaint.complaintType} <br />
                            <strong>Text:</strong> {complaint.complaintText}
                        </li>
                    ))}
                </ul>
            ) : (
                <p>No complaints found for this post.</p>
            )}
        </div>
    );
}
