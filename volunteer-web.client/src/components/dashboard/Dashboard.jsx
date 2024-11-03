import { useEffect, useState } from "react";

// Enum mapping for categories
const categoryNames = {
    0: "Education",
    1: "Environment",
    2: "Health",
    3: "Community",
    4: "Animal Welfare",
};

export default function Dashboard() {
    const [activities, setActivities] = useState([]);

    useEffect(() => {
        const fetchActivities = async () => {
            try {
                const response = await fetch("https://localhost:7149/Posts/dashboard", {
                    method: "POST", // Corrected to POST
                    credentials: "include", // Includes cookies and session data
                    headers: {
                        "Content-Type": "application/json",
                    },
                });

                if (response.ok) {
                    const data = await response.json();
                    setActivities(data);
                } else {
                    console.error("Failed to fetch activities");
                }
            } catch (error) {
                console.error("Error fetching activities:", error);
            }
        };

        fetchActivities();
    }, []);

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">Admin Dashboard</h2>
            <h4 className="text-center mb-4">You Are Admin Of Activities</h4>
            {activities.length > 0 ? (
                <div className="row">
                    {activities.map((activity) => (
                        <div key={activity.id} className="col-md-4 mb-4">
                            <div className="card shadow-sm h-100">
                                <div className="card-body">
                                    <h5 className="card-title">{activity.title}</h5>
                                    <p className="card-text"><strong>Category:</strong> {categoryNames[activity.category]}</p>
                                    <p className="card-text"><strong>Location:</strong> {activity.location}</p>
                                    <p className="card-text"><strong>Dates:</strong> {activity.startDate} to {activity.endDate}</p>
                                </div>
                                <div className="card-footer">
                                    <small className="text-muted">Activity ID: {activity.id}</small>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            ) : (
                <div className="text-center">
                    <p className="lead">No activities found.</p>
                </div>
            )}
        </div>
    );
}
