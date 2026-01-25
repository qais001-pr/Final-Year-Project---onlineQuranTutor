import React from 'react'
import AppNavigation from './src/screens/Navigation/AppNavigation'
import { AuthProvider } from './src/context/auth';
import { LogBox } from 'react-native';
if (__DEV__) {
  LogBox.ignoreAllLogs(true);
}
export default function App() {
  return (
    <AuthProvider>
      <AppNavigation />
    </AuthProvider>
  )
}
