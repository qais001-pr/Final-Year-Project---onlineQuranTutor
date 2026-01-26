/* eslint-disable react-native/no-inline-styles */
import { View, Text, StatusBar, Platform, TouchableOpacity, TextInput, ToastAndroid, ActivityIndicator } from 'react-native'
import React, { useState } from 'react'
import Icon from 'react-native-vector-icons/Ionicons'
import Colors from '../../theme/Colors'
import { useRoute } from '@react-navigation/native'
import { Base_URL } from '../../../IpConfig'
import { Modal } from 'react-native-paper'
export default function UpdatePassword({ navigation }) {
    const route = useRoute();
    console.log(route)
    const { userid } = route.params;
    const [password, setPassword] = useState('12345678')
    const [confirmPassword, setconfirmPassword] = useState('12345678')
    const [loading, setloading] = useState(false)
    const updatePassword = async () => {
        if (!password || !confirmPassword) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please fill all fields', ToastAndroid.LONG);
            return;
        }

        if (password !== confirmPassword) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Passwords do not match', ToastAndroid.LONG);
            return;
        }

        if (password.length < 6) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Password must be at least 6 characters', ToastAndroid.LONG);
            return;
        }
        setloading(true)
        try {
            const response = await fetch(`${Base_URL}auth/update-password?userid=${userid}&password=${password}`, { method: 'POST' });

            const data = await response.json();
            console.log(data);

            if (!response.ok) {
                Platform.OS === 'android' &&
                    ToastAndroid.show(
                        'Failed to update password',
                        ToastAndroid.LONG
                    );
                return;
            }

            Platform.OS === 'android' &&
                ToastAndroid.show('Password updated successfully', ToastAndroid.LONG);
            navigation.replace('Login')
        } catch (error) {
            console.error('Update password error:', error);
            Platform.OS === 'android' &&
                ToastAndroid.show('Something went wrong. Try again.', ToastAndroid.LONG);
        }
        finally {
            setPassword('');
            setconfirmPassword('');
            setloading(false);
        }
    };

    return (
        <View style={{ flex: 1, marginTop: Platform.OS === 'android' && StatusBar.currentHeight, backgroundColor: Colors.backgroundColor }}>
            <StatusBar barStyle={'dark-content'} />
            <View style={{ padding: 20, backgroundColor: Colors.header, flexDirection: 'row' }} >
                <TouchableOpacity onPress={() => navigation.replace('Login')}>
                    <Icon name='arrow-back' size={25} color='white' />
                </TouchableOpacity>
                <Text style={{ fontSize: 20, paddingLeft: 10, color: 'white' }}>Update Password</Text>
            </View>
            <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center', gap: 15 }}>
                <TextInput style={{
                    borderWidth: 2,
                    borderRadius: 10,
                    borderColor: Colors.border,
                    backfaceVisibility: 'hidden',
                    paddingLeft: 10, backgroundColor: 'white', width: '90%'
                }}
                    secureTextEntry
                    placeholder='Password'
                    placeholderTextColor='black'
                    value={password}
                    onChangeText={setPassword}
                />
                <TextInput style={{
                    borderWidth: 2,
                    borderRadius: 10,
                    borderColor: Colors.border,
                    backfaceVisibility: 'hidden',
                    paddingLeft: 10, backgroundColor: 'white', width: '90%'
                }}
                    secureTextEntry
                    placeholder='Confirm Password'
                    placeholderTextColor='black'
                    value={confirmPassword}
                    onChangeText={setconfirmPassword}
                />
                <TouchableOpacity
                    onPress={updatePassword}
                    disabled={loading}
                    style={{ backgroundColor: Colors.button, padding: 10, width: '50%', elevation: 10, borderRadius: 10 }}>
                    <Text style={{ fontSize: 17, textAlign: 'center' }}>
                        Update Password
                    </Text>
                </TouchableOpacity>
            </View>
            <Modal visible={loading} style={{ flex: 1, alignItems: 'center', justifyContent: 'center' }}>
                <ActivityIndicator size={20} color={'#dddd'} />
            </Modal>
        </View>
    )
}