import 'bootstrap/dist/css/bootstrap.min.css';
import { useEffect, useState } from 'react';

export default function About() {
    const [pageContent, setPageContent] = useState('');

    useEffect(() => {
     
        fetch('https://localhost:7149/Home/AboutPage') 
            .then((response) => response.text())
            .then((html) => setPageContent(html))
            .catch((error) => console.error("Error fetching page content:", error));
    }, []);

    return (
        <div className="container mt-5">

            <div dangerouslySetInnerHTML={{ __html: pageContent }} /> {/*for rendering  */}
        </div>
    );
}
