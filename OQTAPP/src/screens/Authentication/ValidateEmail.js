/* eslint-disable react-native/no-inline-styles */
import React, { useState } from 'react'
import { View, Text, StatusBar, TouchableOpacity, TextInput, Platform, ToastAndroid, ActivityIndicator } from 'react-native'
import Icon from 'react-native-vector-icons/Ionicons'
import { Base_URL } from '../../../IpConfig'
import { Modal } from 'react-native-paper'
const Colors = require('../../theme/Colors').default

export default function ValidateEmail({ navigation, route }) {
    const [email, setemail] = useState('ahmerawais123@gmail.com')
    const [loading, setloading] = useState(false)

    const onCheck = async () => {
        if (!email) {
            Platform.OS === 'android' && ToastAndroid.show('Please fill the form first!.', 4000)
            return;
        }
        else {
            setloading(true)
            try {
                const response = await fetch(`${Base_URL}auth/validate-email?email=${email}`)
                // console.log(response);
                const data = await response.json();
                console.log(data)
                if (response.ok) {
                    navigation.replace('UpdatePassword', { userid: data?.userid })
                }
                else {
                    Platform.OS === 'android' && ToastAndroid.show('Invalid Email!.', 4000)
                    return;
                }
            } catch (error) {
                Platform.OS === 'android' && ToastAndroid.show('Error!.Something Went Wrong!', 2000)
                return;

            } finally {
                setloading(false)
                setemail('')
            }
        }
    }

    return (
        <View style={{ flex: 1, marginTop: StatusBar.currentHeight, backgroundColor: Colors.backgroundColor }}>
            <StatusBar barStyle={'dark-content'} />
            <View style={{ padding: 20, backgroundColor: Colors.header, flexDirection: 'row' }} >
                <TouchableOpacity onPress={() => navigation.goBack()}>
                    <Icon name='arrow-back' size={25} color='white' />
                </TouchableOpacity>
                <Text style={{ fontSize: 20, paddingLeft: 10, color: 'white' }}>ValidateEmail</Text>
            </View>
            <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center', gap: 15 }}>
                <TextInput style={{
                    borderWidth: 2,
                    borderRadius: 10,
                    borderColor: Colors.border,
                    backfaceVisibility: 'hidden',
                    paddingLeft: 10, backgroundColor: 'white', width: '90%'
                }}
                    placeholder='Enter Email'
                    placeholderTextColor='black'
                    value={email}
                    onChangeText={setemail}
                />
                <TouchableOpacity
                    onPress={onCheck}
                    disabled={loading}
                    style={{ backgroundColor: Colors.button, padding: 10, width: '50%', elevation: 10, borderRadius: 10 }}>
                    <Text style={{ fontSize: 17, textAlign: 'center' }}>
                        Check
                    </Text>
                </TouchableOpacity>
            </View>
            <Modal visible={loading} style={{ justifyContent: 'center', alignItems: 'center', flex: 1 }}>
                <ActivityIndicator size={20} color={'#ddd'} />
            </Modal>
        </View >
    )
}