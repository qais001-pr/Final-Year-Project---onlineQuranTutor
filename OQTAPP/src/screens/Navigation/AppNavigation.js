import React from 'react';
import { createStackNavigator } from '@react-navigation/stack';
// Screens
import SplashScreen from '../Splash/SplashScreen';
import SignupScreenStudent from '../Authentication/StudentSignupScreen'
import SignupScreenTutor from '../Authentication/TutorSignupScreen'
import LoginScreen from "../Authentication/LoginScreen";
import StudentDashboard from "../Student/StudentDashboard";
import ValidateEmail from "../Authentication/ValidateEmail";
import UpdatePassword from "../Authentication/UpdatePassword";
import TutorDashboard from "../Tutor/TutorDashboard";
import { NavigationContainer } from '@react-navigation/native';
import ProfileScreen from '../Student/ProfileScreen'
const Stack = createStackNavigator();

export default function AppNavigation() {
    return (
        <NavigationContainer>
            <Stack.Navigator
                initialRouteName="Splash"
                screenOptions={{ headerShown: false }}
            >
                <Stack.Screen name="Splash" component={SplashScreen} />
                <Stack.Screen name="Login" component={LoginScreen} />
                <Stack.Screen name="StudentSignup" component={SignupScreenStudent} />
                <Stack.Screen name="TutorSignup" component={SignupScreenTutor} />
                <Stack.Screen name="ValidateEmail" component={ValidateEmail} />
                <Stack.Screen name="UpdatePassword" component={UpdatePassword} />
                <Stack.Screen name="StudentDashboard" component={StudentDashboard} />
                <Stack.Screen name="TutorDashboard" component={TutorDashboard} />


                {/* Student Screen */}
                <Stack.Screen name="StudentProfile" component={ProfileScreen} />
            
            
            </Stack.Navigator>
        </NavigationContainer>
    );
}


