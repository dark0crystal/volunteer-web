import { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import DisplayComplaints from "../card/Complaints";

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
    const [complaintCounts, setComplaintCounts] = useState({});
    const [error, setError] = useState(null);
    const [editingActivity, setEditingActivity] = useState(null);

    useEffect(() => {
        const fetchActivities = async () => {
            try {
                const response = await fetch("https://localhost:7149/Posts/dashboard", {
                    method: "POST",
                    credentials: "include",
                    headers: { "Content-Type": "application/json" },
                });

                if (response.ok) {
                    const data = await response.json();
                    setActivities(data);
                } else {
                    console.error("Failed to fetch activities");
                }
            } catch (error) {
                setError("Error fetching activities. Please try again later.");
            }
        };

        const fetchComplaints = async () => {
            try {
                const response = await fetch("https://localhost:7149/Complaints/grouped-by-post");
                if (response.ok) {
                    const data = await response.json();
                    const counts = data.reduce((acc, item) => {
                        acc[item.postTitle] = item.complaintCount;
                        return acc;
                    }, {});
                    setComplaintCounts(counts);
                } else {
                    console.error("Failed to fetch complaints");
                }
            } catch (error) {
                console.error("Error fetching complaints:", error.message);
            }
        };

        fetchActivities();
        fetchComplaints();
    }, []);

    const handleDelete = async (id) => {
        const response = await fetch(`https://localhost:7149/Posts/delete/${id}`, {
            method: "DELETE",
            credentials: "include",
            headers: { "Content-Type": "application/json" },
        });

        if (response.ok) {
            setActivities(activities.filter(activity => activity.id !== id));
        } else {
            alert("Failed to delete the post.");
        }
    };

    const handleEdit = (activity) => {
        setEditingActivity(activity); // Set activity to be edited
    };

    const handleUpdate = async (updatedActivity) => {
        try {
            const response = await fetch(`https://localhost:7149/Posts/updatepost/${updatedActivity.id}`, {
                method: "PUT",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(updatedActivity),
            });

            if (response.ok) {
                setActivities(
                    activities.map((activity) =>
                        activity.id === updatedActivity.id ? updatedActivity : activity
                    )
                );
                setEditingActivity(null); // Close the edit form
            } else {
                alert("Failed to update the post.");
            }
        } catch (error) {
            console.error("Error updating post:", error);
        }
    };

    if (error) {
        return (
            <div className="container mt-5">
                <div className="alert alert-danger text-center">{error}</div>
            </div>
        );
    }

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">Admin Dashboard</h2>
            {editingActivity ? (
                <div>
                    <h3>Edit Activity</h3>
                    <form
                        onSubmit={(e) => {
                            e.preventDefault();
                            handleUpdate(editingActivity);
                        }}
                    >
                        <div className="form-group">
                            <label>Title</label>
                            <input
                                type="text"
                                className="form-control"
                                value={editingActivity.title}
                                onChange={(e) => setEditingActivity({ ...editingActivity, title: e.target.value })}
                            />
                        </div>
                        <div className="form-group">
                            <label>Category</label>
                            <select
                                className="form-control"
                                value={editingActivity.category}
                                onChange={(e) => setEditingActivity({ ...editingActivity, category: e.target.value })}
                            >
                                {Object.entries(categoryNames).map(([key, value]) => (
                                    <option key={key} value={key}>
                                        {value}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="form-group">
                            <label>Location</label>
                            <input
                                type="text"
                                className="form-control"
                                value={editingActivity.location}
                                onChange={(e) => setEditingActivity({ ...editingActivity, location: e.target.value })}
                            />
                        </div>
                        <div className="form-group">
                            <label>Start Date</label>
                            <input
                                type="date"
                                className="form-control"
                                value={editingActivity.startDate}
                                onChange={(e) => setEditingActivity({ ...editingActivity, startDate: e.target.value })}
                            />
                        </div>
                        <div className="form-group">
                            <label>End Date</label>
                            <input
                                type="date"
                                className="form-control"
                                value={editingActivity.endDate}
                                onChange={(e) => setEditingActivity({ ...editingActivity, endDate: e.target.value })}
                            />
                        </div>
                        <button type="submit" className="btn btn-primary">
                            Update Activity
                        </button>
                    </form>
                </div>
            ) : (
                <div>
                    <h4 className="text-center mb-4">You Are Admin Of Activities</h4>
                    {activities.length > 0 ? (
                        <div className="row">
                            {activities.map((activity) => (
                                <div key={activity.id} className="col-md-4 mb-4">
                                    <div className="card shadow-sm h-100">
                                        <div className="card-body">
                                            <h5 className="card-title">{activity.title}</h5>
                                            <p className="card-text">
                                                <strong>Category:</strong> {categoryNames[activity.category]}
                                            </p>
                                            <p className="card-text">
                                                <strong>Location:</strong> {activity.location}
                                            </p>
                                            <p className="card-text">
                                                <strong>Dates:</strong> {activity.startDate} to {activity.endDate}
                                            </p>
                                            <p className="card-text">
                                                <strong>Complaints:</strong>{" "}
                                                {complaintCounts[activity.title] ?? 0}
                                            </p>
                                        </div>
                                        <div className="card-footer">
                                            <small className="text-muted">Activity ID: {activity.id}</small>
                                            <button
                                                className="btn btn-primary btn-sm ml-2"
                                                onClick={() => handleEdit(activity)}
                                            >
                                                Edit
                                            </button>
                                            <button
                                                className="btn btn-danger btn-sm ml-2"
                                                onClick={() => handleDelete(activity.id)}
                                            >
                                                Delete
                                            </button>
                                        </div>
                                        <div className="p-3">
                                            <DisplayComplaints postId={activity.id} />
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
            )}
        </div>
    );
}
