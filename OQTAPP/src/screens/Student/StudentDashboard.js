/* eslint-disable react/no-unstable-nested-components */
import React from 'react'
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs'
import Icon from 'react-native-vector-icons/Ionicons';
import StudentHomeDashboard from "../Student/Tab/StudentHomeDashboard";
import Schedule from "./Tab/Schedule";
import History from './Tab/History'
import Classes from "./Tab/Classes";
import Tutor from "./Tab/Tutor";
import Colors from '../../theme/Colors';

const Tab = createBottomTabNavigator()

export default function StudentDashboard() {
    return (
        <Tab.Navigator
            initialRouteName="StudentSchedule"
            screenOptions={({ route }) => ({
                headerShown: false,
                tabBarStyle: {
                    borderColor: 'black',
                    backgroundColor: Colors.backgroundColor,
                    borderRadius: 10,
                    height: 110,
                    maxHeight: 110,
                    justifyContent: 'center',
                },
                tabBarIcon: ({ focused, size, color }) => {
                    let iconName

                    if (route.name === 'StudentHomeDashboard') {
                        iconName = focused ? 'home' : 'home-outline'
                    }
                    if (route.name === 'StudentTutor') {
                        iconName = focused ? 'people' : 'people-outline'
                    }

                    if (route.name === 'StudentSchedule') {
                        iconName = focused ? 'calendar' : 'calendar-outline'
                    }

                    if (route.name === 'StudentClasses') {
                        iconName = focused ? 'book' : 'book-outline'
                    }

                    if (route.name === 'StudentHistory') {
                        iconName = focused ? 'time' : 'time-outline'
                    }

                    return <Icon name={iconName} size={focused ? 27 : 20} color={focused ? '#00da41dd' : '#ffffffdd'} />
                }
            })}
        >
            <Tab.Screen
                name="StudentHomeDashboard"
                component={StudentHomeDashboard}
                options={{
                    tabBarLabel: 'Home',
                    tabBarLabelStyle: { fontSize: 15, color: '#ffffff' }
                }}
            />


            <Tab.Screen
                name="StudentTutor"
                component={Tutor}
                options={{
                    tabBarLabel: 'Tutor',
                    tabBarLabelStyle: { fontSize: 15, color: '#ffffff' }
                }}
            />
            <Tab.Screen
                name="StudentSchedule"
                component={Schedule}
                options={{
                    tabBarLabel: 'Schedule',
                    tabBarLabelStyle: { fontSize: 15, color: '#ffffff' }
                }}
            />


            <Tab.Screen
                name="StudentClasses"
                component={Classes}
                options={{
                    tabBarLabel: 'Classes',
                    tabBarLabelStyle: { fontSize: 15, color: '#ffffff' }
                }}
            />

            <Tab.Screen
                name="StudentHistory"
                component={History}
                options={{
                    tabBarLabel: 'History',
                    tabBarLabelStyle: { fontSize: 15, color: '#ffffff' }
                }}
            />
        </Tab.Navigator>
    )
}
