import { useEffect, useState } from "react";

// Enum mapping for categories
const categoryNames = {
    0: "Education",
    1: "Environment",
    2: "Health",
    3: "Community",
    4: "Animal Welfare",
};

export default function VolunteerDashboard() {
    const [activities, setActivities] = useState([]);

    useEffect(() => {
        const fetchActivities = async () => {
            try {
                const response = await fetch("https://localhost:7149/Posts/user-volunteering", {
                    method: "GET", // Use GET instead of POST
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
           
            <h4 className="text-center mb-4">You Are Volunteering In These Activities</h4>
            {activities.length > 0 ? (
                <div className="row">
                    {activities.map((activity) => (
                        <div key={activity.postId} className="col-md-4 mb-4">
                            <div className="card shadow-sm h-100">
                                <div className="card-body">
                                    <h5 className="card-title">{activity.title}</h5>
                                    <p className="card-text"><strong>Category:</strong> {categoryNames[activity.category]}</p>
                                    <p className="card-text"><strong>Organization:</strong> {activity.orgName}</p>
                                    <p className="card-text"><strong>Location:</strong> {activity.location}</p>
                                    <p className="card-text"><strong>Dates:</strong> {activity.startDate} to {activity.endDate}</p>
                                    <p className="card-text"><strong>Number of Days:</strong> {activity.numOfDays}</p>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            ) : (
                <p className="text-center">No volunteering activities found.</p>
            )}
        </div>
    );
}
