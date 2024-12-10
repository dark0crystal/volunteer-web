import { useEffect, useState } from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import { NavLink } from "react-router-dom";
// Import images for categories
import vol2 from "../../assets/vol2.jpeg";
import vol3 from "../../assets/vol3.jpeg";
import vol4 from "../../assets/vol4.jpeg";
import vol5 from "../../assets/vol5.jpeg";
import vol6 from "../../assets/vol6.jpeg";

// Sample category images
const categoryImages = {
    "Education": vol6,
    "Health": vol2,
    "Environment": vol5,
    "Animal Welfare": vol4,
    "Social Services": vol3,
};

export default function Card() {
    const [posts, setPosts] = useState([]);

    useEffect(() => {
        // Fetch posts when the component mounts
        const fetchPosts = async () => {
            try {
                const res = await fetch("https://localhost:7149/Posts/posts");
                const posts = await res.json();
                setPosts(posts);
            } catch (error) {
                console.error("Failed to fetch posts:", error);
            }
        };

        fetchPosts();
    }, []);

    const getCategoryImage = (post) => {
        const categories = ["Education", "Health", "Environment", "Animal Welfare", "Social Services"];
        const category = categories[Math.floor(Math.random() * categories.length)];
        return categoryImages[category];
    };

    return (
        <div className="container mt-5">
            <div className="row">
                {posts.slice(0, 10).map((post) => (
                    <div className="col-md-4 mb-4" key={post.id}>
                        <div className="card h-100 shadow-sm">
                            <img src={getCategoryImage(post)} className="card-img-top" alt="Category" />
                            <div className="card-body">
                                <h5 className="card-title">{post.title}</h5>
                                <h6 className="card-subtitle mb-2 text-muted">{post.description}</h6>
                                <p className="card-text">{post.location}</p>
                                <p className="card-text">{post.postAdminEmail}</p>
                                <NavLink to={`/volunteer-posts/${post.id}`} className="btn btn-primary mb-2">
                                    View Details
                                </NavLink>
                                {/* New button for Complaints Page */}
                                <NavLink to={`/complaints/${post.id}`} className="btn btn-danger">
                                    Go to Complaints
                                </NavLink>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}
