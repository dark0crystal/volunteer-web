import 'bootstrap/dist/css/bootstrap.min.css';
import  { useEffect, useState } from 'react';

export default function Complaints() {
    const [pageContent, setPageContent] = useState('');

    useEffect(() => {
        fetch('https://localhost:7149/Home/ComplaintsPage') // Fetch Razor page content
            .then((response) => response.text())
            .then((html) => setPageContent(html))
            .catch((error) => console.error("Error fetching page content:", error));
    }, []);

    return (
        <div className="container mt-5">
            <div dangerouslySetInnerHTML={{ __html: pageContent }} /> {/* Render Razor page HTML */}
        </div>
    );
}
