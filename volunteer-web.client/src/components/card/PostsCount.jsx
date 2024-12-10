import  { useState, useEffect } from "react";

const PostsCount = () => {
    const [postCountByCategory, setPostCountByCategory] = useState([]);
    const [error, setError] = useState(null);

    useEffect(() => {
        // Fetch the post counts by category from the API
        const fetchPostCounts = async () => {
            try {
                const response = await fetch("https://localhost:7149/Posts/count-posts-by-category"); // Update with your actual endpoint
                if (response.ok) {
                    const data = await response.json();
                    setPostCountByCategory(data);
                } else {
                    setError("Failed to fetch post counts.");
                }
            } catch (error) {
                setError("An error occurred while fetching post counts.");
            }
        };

        fetchPostCounts();
    }, []);

    return (
        <div>
            <h3>Posts Count by Category</h3>
            {error && <p>{error}</p>}
            {postCountByCategory.length > 0 ? (
                <ul>
                    {postCountByCategory.map((item,index) => (
                        <li key={index}>
                            <strong>{item.category}:</strong> {item.postCount} posts
                        </li>
                    ))}
                </ul>
            ) : (
                <p>No data available.</p>
            )}
        </div>
    );
};

export default PostsCount;
