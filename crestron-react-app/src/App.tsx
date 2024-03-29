import React, { useState } from "react";
import "./App.css";
import Button from "react-bootstrap/Button";
import "bootstrap/dist/css/bootstrap.min.css";
import Slider from "@mui/material/Slider";
import {
  bridgeReceiveIntegerFromNative,
  bridgeReceiveBooleanFromNative,
  bridgeReceiveStringFromNative,
  bridgeReceiveObjectFromNative,
} from "@crestron/ch5-crcomlib";

import { publishEvent } from "@crestron/ch5-crcomlib";
import {
  useSubscribeAnalog,
  useSubscribeSerial,
} from "@norgate-av/react-hooks";

(window as any)["bridgeReceiveIntegerFromNative"] =
  bridgeReceiveIntegerFromNative;
(window as any)["bridgeReceiveBooleanFromNative"] =
  bridgeReceiveBooleanFromNative;
(window as any)["bridgeReceiveStringFromNative"] =
  bridgeReceiveStringFromNative;
(window as any)["bridgeReceiveObjectFromNative"] =
  bridgeReceiveObjectFromNative;

function App() {
  const opacityValue = useSubscribeAnalog("Room.Opacity_fb");
  const textResponse = useSubscribeSerial("Room.Text_fb");

  const [color, setColor] = useState("253, 67, 67");
  const [opacity, setOpacity] = useState(1);
  const [background, setBackground] = useState("rgba(253, 67, 67, 1");

  const setSliderValue = (value: number) => {
    setOpacity(value / 100);
    setBackground(`rgba(${color}, ${value / 100})`);
    publishEvent("n", "Room.Opacity_Value", value);
    return `${value}`;
  };

  const setRedBackground = () => {
    setColor("253, 67, 67");
    setBackground(`rgba(253, 67, 67, ${opacity})`);
    publishEvent("b", "Buttons.Red_Pressed", true);
    publishEvent("b", "Buttons.Red_Pressed", false);
  };

  const setGreenBackground = () => {
    setColor("132, 202, 19");
    setBackground(`rgba(132, 202, 19, ${opacity})`);
    publishEvent("b", "Buttons.Green_Pressed", true);
    publishEvent("b", "Buttons.Green_Pressed", false);
  };

  const setYellowBackground = () => {
    setColor("232, 213, 44");
    setBackground(`rgba(232, 213, 44, ${opacity})`);
    publishEvent("b", "Buttons.Yellow_Pressed", true);
    publishEvent("b", "Buttons.Yellow_Pressed", false);
  };

  return (
    <div className="App">
      <h1>React Playground</h1>
      <div className="Button-list" style={{ backgroundColor: background }}>
        <p>{textResponse === "" ? "Red Background Color" : textResponse}</p>
        <Button className="Button" size="lg" onClick={setRedBackground}>
          Red
        </Button>
        <Button className="Button" size="lg" onClick={setGreenBackground}>
          Green
        </Button>
        <Button className="Button" size="lg" onClick={setYellowBackground}>
          Yellow
        </Button>
      </div>
      <div className="Slider">
        <p>Opacity: {textResponse === "" ? "100" : opacityValue}%</p>
        <Slider
          sx={{ width: 800 }}
          defaultValue={100}
          getAriaValueText={setSliderValue}
        ></Slider>
      </div>
    </div>
  );
}

export default App;
