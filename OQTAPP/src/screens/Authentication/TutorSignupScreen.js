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
  KeyboardAvoidingView,
  Platform,
  ActivityIndicator, Modal
} from 'react-native';
import React, { useState, useMemo } from 'react';
import { launchImageLibrary } from 'react-native-image-picker';
import DatePicker from 'react-native-date-picker';
import { Checkbox, RadioButton } from 'react-native-paper';
import { Country, City } from 'country-state-city';
const Colors = require('../../theme/Colors').default;
import { Dropdown } from 'react-native-element-dropdown';
import Icon from 'react-native-vector-icons/Ionicons';
import { Base_URL } from '../../../IpConfig';
import { useAuth } from '../../context/auth';


export default function TutorSignupScreen({ navigation }) {

  const { login } = useAuth()

  const [profile, setpicture] = useState()
  const [name, setName] = useState('Rehan Ali')
  const [date, setDate] = useState(new Date())
  const [email, setEmail] = useState('rehan123@gmail.com')
  const [gender, setGender] = useState('Male')
  const [country, setCountry] = useState('')
  const [city, setCity] = useState(null);
  const [timezone, setTimezone] = useState()
  const [timezonelist, setTimezoneList] = useState([]);
  const [subjects, setSubjectsList] = useState([])
  const [password, setPassword] = useState('1234567890')
  const [confirmpassword, setConfirmPassword] = useState('1234567890')


  const [open, setOpen] = useState(false)
  const [cities, setCities] = useState([]);
  const [nazra, setNazraSubject] = useState(false)
  const [hifz, setHifzSubject] = useState(false)
  const [tajweed, setTajweedSubject] = useState(false)

  const [loading, setLoading] = useState(false)

  const countriesList = useMemo(() => {
    return Country.getAllCountries();
  }, []);
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
          var type = Response.assets[0].type;

          if (type === 'image/png' || type === 'image/jpeg') {
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

  const validate = () => {
    if (!name?.trim()) return false;
    if (!email?.trim()) return false;
    if (!gender) return false;
    if (!country) return false;
    if (!city) return false;
    if (!timezone) return false;
    if (!password) return false;
    if (!confirmpassword) return false;
    if (password !== confirmpassword) return false;
    if (!profile) return false;
    if (!(date instanceof Date)) return false;
    if (!subjects) return false;

    return true;
  };
  const onChangeSubject = ({ subject, name, setsubject }) => {
    setSubjectsList(prev => {
      const exists = prev.some(s => s.name === name);
      return exists
        ? prev.filter(s => s.name !== name)
        : [...prev, { name: name }];
    });
    setsubject(!subject)
  }


  const onSignup = async () => {
    try {
      const isValid = validate();

      if (!isValid) {
        Platform.OS === 'android' &&
          ToastAndroid.show('Please fill all required fields correctly', 2000);
        return;
      }
      setLoading(true)
      const formdata = new FormData();
      const payload = {
        name: name,
        email: email,
        password: password,
        gender: gender,
        dateOfBirth: date.toLocaleDateString(),
        userType: 'Tutor',
        country: country,
        city: city,
        timezone: timezone,
        subjectList: subjects,
      }
      const image = {
        uri: profile,
        type: 'image/png',
        name: 'profile.png'
      }
      formdata.append('tutorImage', image);
      formdata.append('tutor', JSON.stringify(payload));
      console.log(formdata);
      const url = 'Tutor/addTutor';
      const response = await fetch(Base_URL + url, { method: 'POST', body: formdata });
      const data = await response.json();
      if (!data.success) {
        Platform.OS === 'android' && ToastAndroid.show('Email Already Exists', ToastAndroid.BOTTOM)
        return;
      }
      if (response.ok) {
        await login(data?.data);
        setName('');
        setCountry('');
        setCity('');
        setDate(new Date());
        setGender('');
        setNazraSubject(false);
        setTajweedSubject(false);
        setHifzSubject(false);
        setPassword('');
        setConfirmPassword('');
        Platform.OS === 'android' && ToastAndroid.show('Signup successfull.', 4000)
        navigation.replace('TutorDashboard');
      }
      if (!response.ok) {
        var json = await response.json();
        Platform.OS === 'android' && ToastAndroid.show(json.message || 'SignUp Failed!....', 4000)
      }

    } catch (error) {
      Platform.OS === 'android' &&
        ToastAndroid.show('Something went wrong. Please try again.', ToastAndroid.SHORT);
    } finally {
      setLoading(false)
    }
  };
  const onCountryChange = (item) => {
    console.log(item)
    setCountry(item.name);
    console.log(country);
    const cityList = City.getCitiesOfCountry(item.isoCode) || [];
    const FormattedCityList = cityList.map(tz => ({
      ...tz,
      label: `${tz.name} (${tz.stateCode})`,
    }));
    setCities(FormattedCityList);
    const countrys = Country.getAllCountries();
    const formattedCountry = countrys.filter((pk) => pk.name === item.name);
    const timezoneList = formattedCountry[0].timezones;
    const countryTimeZoneList = timezoneList.map(cL => ({ ...cL, label: `${cL.tzName} (${cL.zoneName})` }));
    setTimezoneList(countryTimeZoneList);
  };
  return (
    <View style={{ flex: 1, marginTop: Platform.OS === 'android' ? StatusBar.currentHeight : 10, backgroundColor: Colors.backgroundColor, }}>

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

        <ScrollView style={{ flex: 1 }}
          keyboardShouldPersistTaps='handled'
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
                <View style={{ justifyContent: 'center', alignItems: 'center', width: 150, height: 150, backgroundColor: Colors.border, borderRadius: 100, alignSelf: 'center', marginTop: '10%' }}>
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
            <Text style={{ fontSize: 19, fontWeight: '200', color: 'white' }}>Male</Text>
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
              search
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
              itemTextStyle={{ fontSize: 16 }}
              containerStyle={{ borderRadius: 14, borderColor: Colors.border, borderWidth: 4 }}
            />
            {
              cities.length > 0 &&
              <Dropdown
                data={cities}
                search
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
                itemTextStyle={{ fontSize: 16 }}
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
                itemTextStyle={{ fontSize: 13 }}
                value={timezone}
                onChange={(item) => {
                  setTimezone(item.zoneName);
                }}
              />
            }
          </View>
          <View style={{ flex: 1, marginLeft: 25, marginTop: 15, }}>
            <Text style={{ fontSize: 20, color: 'white', fontWeight: '200' }}>I want to teach</Text>
          </View>
          <View style={{ flex: 1, flexDirection: 'row', marginLeft: 15, marginTop: 5 }}>
            <View style={{ flexDirection: 'row', alignItems: 'center' }}>
              <Checkbox status={nazra ? 'checked' : 'unchecked'}
                onPress={() => onChangeSubject({ subject: nazra, name: 'Nazra', setsubject: setNazraSubject })}
                color='#021b14'
                uncheckedColor='white'
              />
              <Text style={{ fontSize: 19, color: 'white' }}>Nazra</Text>
            </View>
            <View style={{ flexDirection: 'row', alignItems: 'center' }}>
              <Checkbox
                status={tajweed ? 'checked' : 'unchecked'}
                onPress={() => onChangeSubject({ subject: tajweed, name: 'Tajweed', setsubject: setTajweedSubject })}
                color='#021b14'
                uncheckedColor='white'
              />
              <Text style={{ fontSize: 19, color: 'white' }}>Tajweed</Text>
            </View>
            <View style={{ flexDirection: 'row', alignItems: 'center' }}>
              <Checkbox status={hifz ? 'checked' : 'unchecked'}
                onPress={() => onChangeSubject({ subject: hifz, name: 'Hifz', setsubject: setHifzSubject })}
                color='#021b14'
                uncheckedColor='white'
              />
              <Text style={{ fontSize: 19, color: 'white' }}>Hifz</Text>
            </View>
          </View>



          <View style={{ justifyContent: 'center', alignItems: 'center', gap: 15, marginTop: 10 }}>
            <TextInput style={{
              borderWidth: 2,
              borderRadius: 10,
              borderColor: Colors.border,
              fontSize: 15,
              backfaceVisibility: 'hidden',
              paddingLeft: 10, backgroundColor: 'white', width: '90%'
            }}
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
              editable={true}
              placeholder='Enter Confirm Password'
              placeholderTextColor='black'
              value={confirmpassword}
              onChangeText={setConfirmPassword}
            />
            <TouchableOpacity
              onPress={onSignup}
              disabled={loading}
              style={{ borderRadius: 10, backgroundColor: '#ddd', width: '90%', height: 50, justifyContent: 'center', alignItems: 'center' }}>
              <Text style={{ fontSize: 19, fontWeight: '400', color: '#0a7656' }}>Sign Up</Text>
            </TouchableOpacity>
          </View>
          <View style={{ marginBottom: 50, justifyContent: 'center', alignItems: 'center', margin: 10, flexDirection: 'row', gap: 10 }}>
            <Text style={{ justifyContent: 'center', alignItems: 'center', fontSize: 19 }}>Do you have an Account?

            </Text>
            <TouchableOpacity style={{ justifyContent: 'center', alignContent: 'center' }} onPress={() => navigation.replace('Login')}>
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
      <Modal visible={loading} style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
        <ActivityIndicator size={30} color={'#dddd'} />
      </Modal>
    </View >
  )
}