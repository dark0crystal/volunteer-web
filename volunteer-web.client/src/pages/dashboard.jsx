import UserDashboard from "../components/dashboard/Dashboard"
import VolunteerDashboard from "../components/dashboard/VolunteerDashboard"
export default function Dashboard() {
    return (
        <div>
            <VolunteerDashboard/>
            <UserDashboard />
        </div>
    )
}