/* eslint-disable no-shadow */
/* eslint-disable react-native/no-inline-styles */
import {
    StatusBar,
    View,
    Text,
    ScrollView,
    TouchableOpacity,
    ToastAndroid,
    Image,
    TextInput,
    Platform,
    KeyboardAvoidingView,
} from 'react-native'
import React, { useState, useMemo } from 'react';
import { launchImageLibrary } from 'react-native-image-picker';
import DatePicker from 'react-native-date-picker';
import { ActivityIndicator, Modal, RadioButton } from 'react-native-paper';
import { Country, City } from 'country-state-city';
const Colors = require('../../theme/Colors').default;
import { Dropdown } from 'react-native-element-dropdown';
import Icon from 'react-native-vector-icons/Ionicons';
import { Base_URL } from "../../../IpConfig";
import { useAuth } from '../../context/auth';
export default function SignupScreenStudent({ navigation }) {

    const [profile, setpicture] = useState()
    const [name, setName] = useState('Ahmed Raza');
    const [email, setEmail] = useState('ahmedraza12345@gmail.com');
    const [country, setCountry] = useState('');
    const [date, setDate] = useState(new Date());
    const [open, setOpen] = useState(false);
    const [gender, setGender] = useState('Male');
    const [cities, setCities] = useState([]);
    const [city, setCity] = useState(null);
    const [timezonelist, setTimezoneList] = useState([]);
    const [timezone, setTimezone] = useState();
    const [subject, setSubject] = useState('Nazra');
    const [checkGuardian, setCheckGuardian] = useState(false);
    const [preferredTutor, setPrefferedTutor] = useState();
    const [password, setPassword] = useState('123456789');
    const [confirmpassword, setConfirmPassword] = useState('123456789');
    const [loading, setloading] = useState(false);
    const { login } = useAuth();

    const countriesList = useMemo(() => {
        return Country.getAllCountries();
    }, []);

    const preffered_list = [
        { label: 'Male', value: 'Male' },
        { label: 'Female', value: 'Female' },
        { label: 'Both', value: 'Both' }
    ];


    const selectImage = () => {
        try {
            launchImageLibrary({
                mediaType: 'photo',
                quality: 1,
                includeBase64: false,
            }, Response => {
                if (Response.didCancel) {
                    Platform.OS === 'android' && ToastAndroid.show('Operation Cancel', 4000);
                    return;
                }
                if (Response.errorCode || Response.errorMessage) {
                    Platform.OS === 'android' && ToastAndroid.show('Something Went Wrong!.Error Occurred.', 4000);
                    return;
                }
                if (Response.assets && Response.assets.length > 0) {
                    var image = Response.assets[0].uri;
                    if (Response.assets[0].type === 'image/png'
                        ||
                        Response.assets[0].type === 'image/jpeg') {
                        setpicture(image)
                        Platform.OS === 'android' && ToastAndroid.show('Image Selected Successfully', 2000);
                        return;
                    }
                    else {
                        Platform.OS === 'android' && ToastAndroid.show('Invalid Image Type!...', 4000);
                        return;
                    }
                }
            })
        } catch (e) {
            Platform.OS === 'android' && ToastAndroid.show('Error Occurred!.', 4000)
        }
    }
    const onCountryChange = (item) => {
        setCountry(item.name);
        const cityList = City.getCitiesOfCountry(item.isoCode) || [];
        const FormattedCityList = cityList.map(tz => ({
            ...tz,
            label: `${tz.name} (${tz.stateCode})`,
        }));
        setCities(FormattedCityList);
        const country = Country.getAllCountries();
        const formattedCountry = country.filter((pk) => pk.name === item.name);
        const timezoneList = formattedCountry[0].timezones;
        const countryTimeZoneList = timezoneList.map(cL => ({ ...cL, label: `${cL.tzName} (${cL.zoneName})` }));
        setTimezoneList(countryTimeZoneList);
    };

    const validate = () => {
        if (!name?.trim()) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please fill the fullname', ToastAndroid.SHORT);
            return false;
        }
        if (!email?.trim()) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please fill the email', ToastAndroid.SHORT);
            return false;
        }
        if (!gender) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please select the gender', ToastAndroid.SHORT);
            return false;
        }
        if (!country) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please select the country', ToastAndroid.SHORT);
            return false;
        }
        if (!city) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please select the city', ToastAndroid.SHORT);
            return false;
        }
        if (!timezone) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please select the timezone', ToastAndroid.SHORT);
            return false;
        }

        if (!preferredTutor) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please select the preffered tutor', ToastAndroid.SHORT);
            return false;
        }
        if (!password) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please fill the password', ToastAndroid.SHORT);
            return false;
        }
        if (!confirmpassword) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Please fill the confirm password', ToastAndroid.SHORT);
            return false;
        }
        if (password !== confirmpassword) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Password are not same', ToastAndroid.SHORT);
            return false;
        }
        if (!profile) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Select Profile Picture', ToastAndroid.SHORT);
            return false;
        }
        if (!(date instanceof Date)) {
            Platform.OS === 'android' &&
                ToastAndroid.show('Select date of birth', ToastAndroid.SHORT);
            return false;
        }
        return true;
    };

    const onSignup = async () => {
        try {
            const isValid = validate();

            if (!isValid) {
                return;
            }
            setloading(true)
            const formdata = new FormData();
            const payload = {
                name: name,
                email: email,
                password: password,
                gender: gender,
                dateOfBirth: date.toLocaleDateString(),
                userType: checkGuardian ? 'Guardian' : 'Student',
                country: country,
                city: city ? city : country,
                preferred_tutor: preferredTutor,
                timezone: timezone,
                subject: subject
            }
            const image = {
                uri: profile,
                type: 'image/png',
                name: 'profile.png'
            }
            formdata.append('image', image);
            formdata.append('data', JSON.stringify(payload));
            console.log(formdata);
            const url = checkGuardian ? 'Guardian/addGuardian' : 'Students/addStudent';
            const response = await fetch(Base_URL + url, { method: 'POST', body: formdata });
            console.log(response);
            const data = await response.json();
            // console.log(data);
            // console.log(data?.data);
            if (!data.success) {
                Platform.OS === 'android' && ToastAndroid.show('Email Already Exists', ToastAndroid.BOTTOM)
                return;
            }
            if (response.ok) {
                await login(data?.data);
                Platform.OS === 'android' && ToastAndroid.show('Signup successfull.', 4000)
                navigation.replace(checkGuardian ? 'GuardianDashboard' : 'StudentDashboard')
            }
            if (!response.ok) {
                var json = await response.json();
                Platform.OS === 'android' && ToastAndroid.show(json.message || 'SignUp Failed!....', 4000)
            }
            setName('');
            setCountry('');
            setCity('');
            setTimezone('');
            setDate(new Date());
            setGender('Male');
            setSubject('Nazra')
            setPassword('');
            setConfirmPassword('');

        } catch (error) {
            console.log(error.message)
            Platform.OS === 'android' &&
                ToastAndroid.show('Something went wrong. Please try again.', ToastAndroid.SHORT);
        } finally {
            setloading(false);
        }
    };

    return (
        <View style={{ flex: 1, marginTop: Platform.OS === 'android' ? StatusBar.currentHeight : 0, backgroundColor: Colors.backgroundColor, }}>

            <StatusBar backgroundColor={Colors.primary} barStyle="dark-content" />
            <View style={{ flexDirection: 'row', flex: 0.07, justifyContent: 'center', alignItems: 'center', backgroundColor: Colors.header }}>
                <TouchableOpacity style={{ justifyContent: 'flex-start', padding: 10 }} onPress={() => navigation.goBack()}>
                    <Icon name="arrow-back" size={26} color="#ffffff" />
                </TouchableOpacity>

                <View style={{ flex: 1 }}>
                    <Text style={{ color: '#fff', fontSize: 25, fontWeight: 'bold' }}>Create Account</Text>
                </View>
            </View>
            <KeyboardAvoidingView
                style={{ flex: 1 }}
                behavior={Platform.OS === 'android' ? 'padding' : 'height'}>

                <ScrollView
                    style={{ flex: 1 }}
                    keyboardShouldPersistTaps="handled"
                    contentContainerStyle={{ flexGrow: 1 }}>
                    <View style={{ justifyContent: 'center', alignItems: 'center' }}>
                        {
                            profile != null ? (

                                <Image
                                    borderRadius={100}
                                    width={150}
                                    height={150}
                                    style={{ justifyContent: 'center', alignItems: 'center', alignSelf: 'center', marginTop: '10%', }}
                                    source={{ uri: profile }}
                                />
                            ) : (
                                <View style={{ justifyContent: 'center', alignItems: 'center', width: 150, borderRadius: 100, height: 150, backgroundColor: Colors.border, alignSelf: 'center', marginTop: '10%' }}>
                                    <Text style={{ fontSize: 20, fontWeight: '300' }}>No Image</Text>
                                </View>
                            )
                        }
                        <TouchableOpacity
                            onPress={selectImage}
                            style={{ width: 100, height: 40, backgroundColor: Colors.button, elevation: 4, margin: 10, justifyContent: 'center', alignItems: 'center', borderRadius: 10 }}>
                            <Text style={{ color: 'white', fontSize: 20 }}>
                                Edit
                            </Text>
                        </TouchableOpacity>
                    </View>
                    <View style={{ justifyContent: 'center', alignItems: 'center', gap: 15 }}>
                        <TextInput style={{
                            borderWidth: 2,
                            borderRadius: 10,
                            borderColor: Colors.border,
                            backfaceVisibility: 'hidden',
                            paddingLeft: 10, backgroundColor: 'white', width: '90%'
                        }}
                            placeholder='Enter Name'
                            placeholderTextColor='black'
                            value={name}
                            onChangeText={setName}
                        />
                        <TouchableOpacity style={{
                            flexDirection: 'row',
                            justifyContent: 'space-between',
                            height: 45,
                            borderWidth: 2,
                            borderRadius: 10,
                            borderColor: Colors.border,
                            backfaceVisibility: 'hidden',
                            paddingLeft: 10, backgroundColor: 'white', width: '90%'
                        }}
                            onPress={() => setOpen(true)}
                        >
                            <Text style={{ alignSelf: 'center', fontSize: 15 }}>{date.toLocaleDateString() === new Date().toLocaleDateString() ? 'Select date of birth' : date.toLocaleDateString()}</Text>
                        </TouchableOpacity>
                        <TextInput style={{
                            borderWidth: 2,
                            borderRadius: 10,
                            borderColor: Colors.border,
                            paddingLeft: 10, backgroundColor: 'white', width: '90%'
                        }}
                            placeholder='Enter Email'
                            placeholderTextColor='black'
                            value={email}
                            onChangeText={setEmail}
                        />
                    </View>
                    <View style={{ flexDirection: 'row', justifyContent: 'flex-start', margin: 20, marginLeft: 25 }}>
                        <Text style={{ fontSize: 19, fontWeight: '700', color: 'white' }}>Gender</Text>
                        <RadioButton status={gender === 'Male' ? 'checked' : 'unchecked'}
                            onPress={() => setGender('Male')}
                            color='#021b14'
                            uncheckedColor='white'
                        />
                        <Text style={{ fontSize: 19, fontWeight: '700', color: 'white' }}>Male</Text>
                        <RadioButton status={gender === 'Female' ? 'checked' : 'unchecked'}
                            onPress={() => setGender('Female')}
                            color='#021b14'
                            uncheckedColor='white'
                        />
                        <Text style={{ fontSize: 19, fontWeight: '700', color: 'white' }}>Female</Text>
                    </View>

                    <View style={{ gap: 15, justifyContent: 'center', alignItems: 'center' }}>
                        <Dropdown
                            data={countriesList}
                            search={true}
                            style={{
                                width: '90%',
                                height: 45,
                                borderRadius: 10,
                                borderWidth: 2,
                                backgroundColor: 'white',
                                padding: 10,
                                borderColor: '#dddd'
                            }}
                            labelField="name"
                            valueField="isoCode"
                            placeholder="Select Country"
                            onChange={onCountryChange}
                        />

                        {
                            cities.length > 0 &&
                            < Dropdown
                                data={cities}
                                search={true}
                                style={{
                                    width: '90%',
                                    height: 45,
                                    borderRadius: 10,
                                    borderWidth: 2,
                                    backgroundColor: 'white',
                                    padding: 10,
                                    borderColor: '#dddd'
                                }}
                                labelField="label"
                                valueField="name"
                                placeholder="Select City"
                                value={city}
                                onChange={(item) => {
                                    setCity(item.name);
                                }}
                            />
                        }
                        {
                            timezonelist.length > 0 &&
                            < Dropdown
                                data={timezonelist}
                                style={{
                                    width: '90%',
                                    height: 45,
                                    borderRadius: 10,
                                    borderWidth: 2,
                                    backgroundColor: 'white',
                                    padding: 10,
                                    borderColor: '#dddd'
                                }}
                                labelField="label"
                                valueField="zoneName"
                                placeholder="Select Timezone"
                                value={timezone}
                                onChange={(item) => {
                                    setTimezone(item.zoneName);
                                }}
                            />
                        }

                    </View>
                    <View style={{ flex: 1, marginLeft: 25, marginTop: 15, }}>
                        <Text style={{ fontSize: 20, color: 'white', fontWeight: '200' }}>I want to learn</Text>
                    </View>
                    <View style={{ flex: 1, flexDirection: 'row', marginLeft: 15, marginTop: 5 }}>
                        <View style={{ flexDirection: 'row', alignItems: 'center' }}>
                            <RadioButton status={subject === 'Nazra' ? 'checked' : 'unchecked'} s
                                onPress={() => setSubject('Nazra')}
                                color='#021b14'
                                uncheckedColor='white'
                            />
                            <Text style={{ fontSize: 19, color: 'white' }}>Nazra</Text>
                        </View>
                        <View style={{ flexDirection: 'row', alignItems: 'center' }}>
                            <RadioButton status={subject === 'Tajweed' ? 'checked' : 'unchecked'} s
                                onPress={() => setSubject('Tajweed')}
                                color='#021b14'
                                uncheckedColor='white'
                            />
                            <Text style={{ fontSize: 19, color: 'white' }}>Tajweed</Text>
                        </View>
                        <View style={{ flexDirection: 'row', alignItems: 'center' }}>
                            <RadioButton status={subject === 'Hifz' ? 'checked' : 'unchecked'} s
                                onPress={() => setSubject('Hifz')}
                                color='#021b14'
                                uncheckedColor='white'
                            />
                            <Text style={{ fontSize: 19, color: 'white' }}>Hifz</Text>
                        </View>
                    </View>

                    <View style={{ flex: 1, marginLeft: 25, marginTop: 15, }}>
                        <Text style={{ fontSize: 20, color: 'white', fontWeight: '200' }}>Select Preffered Tutor</Text>
                    </View>

                    <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center', marginTop: 10 }}>
                        <Dropdown
                            data={preffered_list}
                            style={{
                                width: '90%',
                                height: 45,
                                borderRadius: 10,
                                borderWidth: 2,
                                backgroundColor: 'white',
                                padding: 10,
                                borderColor: '#dddd'
                            }}
                            labelField="label"
                            valueField="value"
                            value={preferredTutor}
                            placeholder="Select Preferred Tutor"
                            onChange={item => {
                                const g = item.value;
                                setPrefferedTutor(g);
                            }}
                        />
                    </View>



                    <View style={{ flex: 1, marginLeft: 25, marginTop: 15, }}>
                        <Text style={{ fontSize: 20, color: 'white', fontWeight: '200' }}>Are you a Guardian?</Text>
                    </View>

                    <View style={{ flex: 1, flexDirection: 'row', marginLeft: 20, marginTop: 5 }}>
                        <View style={{ flexDirection: 'row', alignItems: 'center' }}>
                            <RadioButton status={checkGuardian ? 'checked' : 'unchecked'} s
                                onPress={() => setCheckGuardian(true)}
                                color='#021b14'
                                uncheckedColor='white'
                            />
                            <Text style={{ fontSize: 19, color: 'white' }}>Yes</Text>
                        </View>
                        <View style={{ flexDirection: 'row', alignItems: 'center' }}>
                            <RadioButton status={checkGuardian === false ? 'checked' : 'unchecked'} s
                                onPress={() => setCheckGuardian(false)}
                                color='#021b14'
                                uncheckedColor='white'
                            />
                            <Text style={{ fontSize: 19, color: 'white' }}>No</Text>
                        </View>
                    </View>



                    <View style={{ justifyContent: 'center', alignItems: 'center', gap: 15 }}>
                        <TextInput style={{
                            borderWidth: 2,
                            borderRadius: 10,
                            borderColor: Colors.border,
                            fontSize: 15,
                            backfaceVisibility: 'hidden',
                            paddingLeft: 10, backgroundColor: 'white', width: '90%'
                        }}
                            secureTextEntry
                            editable={true}
                            placeholder='Enter Password'
                            placeholderTextColor='black'
                            value={password}
                            onChangeText={setPassword}
                        />
                        <TextInput style={{
                            borderWidth: 2,
                            borderRadius: 10,
                            borderColor: Colors.border,
                            fontSize: 15,
                            backfaceVisibility: 'hidden',
                            paddingLeft: 10, backgroundColor: 'white', width: '90%'
                        }}
                            secureTextEntry
                            editable={true}
                            placeholder='Enter Confirm Password'
                            placeholderTextColor='black'
                            value={confirmpassword}
                            onChangeText={setConfirmPassword}
                        />
                        <TouchableOpacity
                            onPress={onSignup}
                            style={{ borderRadius: 10, backgroundColor: '#ddd', width: '90%', height: 50, justifyContent: 'center', alignItems: 'center' }}>
                            <Text style={{ fontSize: 19, fontWeight: '400', color: '#0a7656' }}>Sign Up</Text>
                        </TouchableOpacity>
                    </View>
                    <View style={{ marginBottom: 50, justifyContent: 'center', alignItems: 'center', margin: 10, flexDirection: 'row', gap: 10 }}>
                        <Text style={{ justifyContent: 'center', alignItems: 'center', fontSize: 19 }}>Do you have an Account?

                        </Text>
                        <TouchableOpacity style={{ justifyContent: 'center', alignContent: 'center' }} onPress={() => navigation.navigate('Login')}>
                            <Text style={{ alignSelf: 'center', fontSize: 17, color: '#ffffffdd', fontWeight: '700' }}>Login</Text>
                        </TouchableOpacity>
                    </View>
                </ScrollView>
            </KeyboardAvoidingView>
            <DatePicker
                modal
                open={open}
                date={date}
                mode='date'
                onConfirm={(date) => {
                    setOpen(false)
                    setDate(date)
                }}
                onCancel={() => {
                    setOpen(false)
                }}
            />
            <Modal
                visible={loading}
                style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
                <ActivityIndicator  size={30} color='#dddddd' />
            </Modal>
        </View >
    )
}