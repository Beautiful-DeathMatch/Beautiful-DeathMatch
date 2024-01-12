/*
FPS 체크해주는 스크립트 소스입니다.
출처: https://linkmemo.tistory.com/213
*/

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FPSCheck : MonoBehaviour
{
    float delta_ = 0f;
    float sec_sum_delta_ = 1f;
    float secs_sum_delta_ = 10f;
    int min_fps_ = 200;
    int max_fps_ = 0;
    int real_fps_ = 0;
    int min_real_fps_ = 500;
    int max_real_fps_ = 0;
    int real_fps_counter_ = 0;
    StringBuilder builder_ = new StringBuilder();
    GUIStyle style_ = new GUIStyle();
    Rect rect_;
    int old_width_ = -1;
    int old_height_ = -1;

    void Update()
    {
        delta_ = Time.unscaledDeltaTime;
        sec_sum_delta_ -= delta_;
        real_fps_counter_++;

        if (sec_sum_delta_ <= 0f)
        {
            secs_sum_delta_ += -1f + sec_sum_delta_;
            if (secs_sum_delta_ <= 0)
            {
                min_real_fps_ = 500;
                max_real_fps_ = 0;
                secs_sum_delta_ = 10f;
            }

            min_fps_ = 200;
            max_fps_ = 0;
            real_fps_ = real_fps_counter_;
            real_fps_counter_ = 0;
            sec_sum_delta_ = 1f;
        }
    }

    private void OnGUI()
    {
        float msec = delta_ * 1000;
        int fps = (int)(1f / delta_);

        if (fps < min_fps_)
        {
            min_fps_ = fps;
        }
        if (max_fps_ < fps)
        {
            max_fps_ = fps;
        }

        if (real_fps_ < min_real_fps_)
        {
            min_real_fps_ = real_fps_;
        }

        if (max_real_fps_ < real_fps_)
        {
            max_real_fps_ = real_fps_;
        }

        builder_.Clear();
        builder_.AppendFormat(" {0:000.} ms \n", msec);
        builder_.AppendFormat(" {0} fps \n", fps);
        builder_.AppendFormat(" {0} min fps \n", min_fps_);
        builder_.AppendFormat(" {0} max fps \n", max_fps_);
        builder_.AppendFormat(" {0} real fps ({1} ~ {2}) \n", real_fps_, min_real_fps_, max_real_fps_);

        int width = Screen.width;
        int height = Screen.height;

        if (width != old_width_ || height != old_height_)
        {
            rect_ = new Rect(width * 0.01f, height * 0.02f, width * 0.1f, height * 0.1f);

            style_.alignment = TextAnchor.UpperLeft;
            //style_.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            style_.fontSize = (int)(width * 0.01f);
            var color = style_.normal.textColor;
            color.r = 0f;
            color.g = 0f;
            color.b = 0f;
            color.a = 1f;
            style_.normal.textColor = color;

            old_width_ = width;
            old_height_ = height;
        }

        GUI.Label(rect_, builder_.ToString(), style_);
    }
}
