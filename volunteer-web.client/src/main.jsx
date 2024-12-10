
import ReactDOM from 'react-dom/client';

import App from './App';

import { BrowserRouter,Route , Routes } from 'react-router-dom';
import Ayhagah from './pages/volunteer-posts';
import About from './pages/about';
import 'bootstrap/dist/css/bootstrap.css';
import NavMenu from './components/navbar/NavMenu';
import Login from './pages/login';
import VolunteerPost from './pages/volunteer-posts';
import Register from './pages/register';
import VolunteeringRegister from './pages/volunteering-register';
import Footer from './components/Footer';
import PostDetails from './components/PostDetails';
import Dashboard from './pages/dashboard';
import Complaints from './pages/complaints';


const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  // <React.StrictMode>
  //   <App />
  // </React.StrictMode>
<BrowserRouter>
<NavMenu/>
  <Routes>
    <Route path='/' element={<App/>}/>
    <Route path='/about' element={<About/>}/>
    <Route path='/ayhagah' element={<Ayhagah/>}/>
    <Route path='/login' element={<Login/>}/>
    <Route path='/register' element={<Register/>}/>
    <Route path='/volunteering-register' element={<VolunteeringRegister/>}/>
    <Route path='/volunteer-posts' element={<VolunteerPost/>}/>
    <Route path="/volunteer-posts/:postId" element={<PostDetails />} />
    <Route path="/dashboard" element={<Dashboard />} />
    <Route path="/complaints/:postId" element={<Complaints />} />
    
  </Routes>
  <Footer/>
</BrowserRouter>
);


